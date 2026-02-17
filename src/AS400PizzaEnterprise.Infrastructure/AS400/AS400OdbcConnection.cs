using System.Data;
using System.Data.Odbc;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AS400PizzaEnterprise.Domain.Interfaces;

namespace AS400PizzaEnterprise.Infrastructure.AS400;

public class AS400OdbcConnection : IAS400Connection, IDisposable
{
    private readonly ILogger<AS400OdbcConnection> _logger;
    private readonly string _connectionString;
    private OdbcConnection? _connection;
    private OdbcTransaction? _transaction;
    private bool _disposed;

    public AS400OdbcConnection(IConfiguration configuration, ILogger<AS400OdbcConnection> logger)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("AS400Connection") 
            ?? throw new InvalidOperationException("AS400Connection connection string is not configured");
    }

    private async Task EnsureConnectionOpenAsync()
    {
        if (_connection == null)
        {
            _connection = new OdbcConnection(_connectionString);
        }

        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();
            _logger.LogDebug("AS400 connection opened");
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            await EnsureConnectionOpenAsync();
            
            using var command = _connection!.CreateCommand();
            command.CommandText = sql;
            command.Transaction = _transaction;
            
            if (parameters != null)
            {
                AddParameters(command, parameters);
            }

            _logger.LogDebug("Executing query: {Sql}", sql);

            var results = new List<T>();
            using var reader = (OdbcDataReader)await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                results.Add(MapFromReader<T>(reader));
            }

            _logger.LogDebug("Query returned {Count} results", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query: {Sql}", sql);
            throw;
        }
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            await EnsureConnectionOpenAsync();
            
            using var command = _connection!.CreateCommand();
            command.CommandText = sql;
            command.Transaction = _transaction;
            
            if (parameters != null)
            {
                AddParameters(command, parameters);
            }

            _logger.LogDebug("Executing query first or default: {Sql}", sql);

            using var reader = (OdbcDataReader)await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var result = MapFromReader<T>(reader);
                _logger.LogDebug("Query returned a result");
                return result;
            }

            _logger.LogDebug("Query returned no results");
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query first or default: {Sql}", sql);
            throw;
        }
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        try
        {
            await EnsureConnectionOpenAsync();
            
            using var command = _connection!.CreateCommand();
            command.CommandText = sql;
            command.Transaction = _transaction;
            
            if (parameters != null)
            {
                AddParameters(command, parameters);
            }

            _logger.LogDebug("Executing command: {Sql}", sql);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            
            _logger.LogDebug("Command affected {RowsAffected} rows", rowsAffected);
            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command: {Sql}", sql);
            throw;
        }
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            await EnsureConnectionOpenAsync();
            
            using var command = _connection!.CreateCommand();
            command.CommandText = sql;
            command.Transaction = _transaction;
            
            if (parameters != null)
            {
                AddParameters(command, parameters);
            }

            _logger.LogDebug("Executing scalar: {Sql}", sql);

            var result = await command.ExecuteScalarAsync();
            
            if (result == null || result == DBNull.Value)
            {
                _logger.LogDebug("Scalar query returned null");
                return default;
            }

            _logger.LogDebug("Scalar query returned: {Result}", result);
            return (T)Convert.ChangeType(result, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing scalar: {Sql}", sql);
            throw;
        }
    }

    public async Task BeginTransactionAsync()
    {
        await EnsureConnectionOpenAsync();
        
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction is already in progress");
        }

        _transaction = _connection!.BeginTransaction();
        _logger.LogDebug("Transaction started");
    }

    public Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction in progress");
        }

        _transaction.Commit();
        _transaction.Dispose();
        _transaction = null;
        _logger.LogDebug("Transaction committed");
        
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction in progress");
        }

        _transaction.Rollback();
        _transaction.Dispose();
        _transaction = null;
        _logger.LogDebug("Transaction rolled back");
        
        return Task.CompletedTask;
    }

    private void AddParameters(OdbcCommand command, object parameters)
    {
        var properties = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        foreach (var property in properties)
        {
            var value = property.GetValue(parameters);
            var parameter = command.CreateParameter();
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }

    private T MapFromReader<T>(OdbcDataReader reader)
    {
        var type = typeof(T);
        
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || 
            type == typeof(DateTime) || type == typeof(Guid))
        {
            var value = reader.GetValue(0);
            if (value == DBNull.Value)
            {
                return default!;
            }
            
            if (type == typeof(Guid) && value is string stringValue)
            {
                return (T)(object)Guid.Parse(stringValue);
            }
            
            return (T)Convert.ChangeType(value, type);
        }

        var instance = Activator.CreateInstance<T>();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        for (int i = 0; i < reader.FieldCount; i++)
        {
            var columnName = reader.GetName(i);
            var property = properties.FirstOrDefault(p => 
                p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

            if (property != null && property.CanWrite)
            {
                var value = reader.GetValue(i);
                
                if (value != DBNull.Value)
                {
                    if (property.PropertyType == typeof(Guid) && value is string guidString)
                    {
                        property.SetValue(instance, Guid.Parse(guidString));
                    }
                    else if (property.PropertyType.IsEnum)
                    {
                        if (value is string enumString)
                        {
                            property.SetValue(instance, Enum.Parse(property.PropertyType, enumString, true));
                        }
                        else if (value is int enumInt)
                        {
                            property.SetValue(instance, Enum.ToObject(property.PropertyType, enumInt));
                        }
                    }
                    else if (property.PropertyType == typeof(bool) && value is string boolString)
                    {
                        property.SetValue(instance, boolString == "1" || 
                            boolString.Equals("true", StringComparison.OrdinalIgnoreCase) || 
                            boolString.Equals("Y", StringComparison.OrdinalIgnoreCase));
                    }
                    else if (property.PropertyType == typeof(bool) && value is int boolInt)
                    {
                        property.SetValue(instance, boolInt == 1);
                    }
                    else
                    {
                        try
                        {
                            var convertedValue = Convert.ChangeType(value, property.PropertyType);
                            property.SetValue(instance, convertedValue);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Could not map column {ColumnName} to property {PropertyName}", 
                                columnName, property.Name);
                        }
                    }
                }
            }
        }

        return instance;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _transaction?.Dispose();
        _connection?.Dispose();
        _disposed = true;
        
        _logger.LogDebug("AS400 connection disposed");
    }
}
