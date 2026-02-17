using AS400PizzaEnterprise.Domain.Entities;
using AS400PizzaEnterprise.Domain.Interfaces;
using AS400PizzaEnterprise.Domain.ValueObjects;
using AS400PizzaEnterprise.Infrastructure.AS400;
using Microsoft.Extensions.Logging;

namespace AS400PizzaEnterprise.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IAS400Connection _connection;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(IAS400Connection connection, IUnitOfWork unitOfWork, ILogger<CustomerRepository> logger)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Customers.Id}, {AS400TableConstants.Customers.FirstName},
                   {AS400TableConstants.Customers.LastName}, {AS400TableConstants.Customers.Email},
                   {AS400TableConstants.Customers.PhoneNumber}, {AS400TableConstants.Customers.DefaultStreet},
                   {AS400TableConstants.Customers.DefaultCity}, {AS400TableConstants.Customers.DefaultState},
                   {AS400TableConstants.Customers.DefaultZipCode}, {AS400TableConstants.Customers.DefaultCountry},
                   {AS400TableConstants.Customers.IsActive}, {AS400TableConstants.Customers.CreatedAt},
                   {AS400TableConstants.Customers.UpdatedAt}
            FROM {AS400TableConstants.CustomersTable}
            WHERE {AS400TableConstants.Customers.Id} = ?";

        var dto = await _connection.QueryFirstOrDefaultAsync<CustomerDto>(sql, new { Id = id.ToString() });
        return dto != null ? MapToEntity(dto) : null;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Customers.Id}, {AS400TableConstants.Customers.FirstName},
                   {AS400TableConstants.Customers.LastName}, {AS400TableConstants.Customers.Email},
                   {AS400TableConstants.Customers.PhoneNumber}, {AS400TableConstants.Customers.DefaultStreet},
                   {AS400TableConstants.Customers.DefaultCity}, {AS400TableConstants.Customers.DefaultState},
                   {AS400TableConstants.Customers.DefaultZipCode}, {AS400TableConstants.Customers.DefaultCountry},
                   {AS400TableConstants.Customers.IsActive}, {AS400TableConstants.Customers.CreatedAt},
                   {AS400TableConstants.Customers.UpdatedAt}
            FROM {AS400TableConstants.CustomersTable}
            ORDER BY {AS400TableConstants.Customers.LastName}, {AS400TableConstants.Customers.FirstName}";

        var dtos = await _connection.QueryAsync<CustomerDto>(sql);
        return dtos.Select(MapToEntity);
    }

    public async Task AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            INSERT INTO {AS400TableConstants.CustomersTable}
            ({AS400TableConstants.Customers.Id}, {AS400TableConstants.Customers.FirstName},
             {AS400TableConstants.Customers.LastName}, {AS400TableConstants.Customers.Email},
             {AS400TableConstants.Customers.PhoneNumber}, {AS400TableConstants.Customers.DefaultStreet},
             {AS400TableConstants.Customers.DefaultCity}, {AS400TableConstants.Customers.DefaultState},
             {AS400TableConstants.Customers.DefaultZipCode}, {AS400TableConstants.Customers.DefaultCountry},
             {AS400TableConstants.Customers.IsActive}, {AS400TableConstants.Customers.CreatedAt},
             {AS400TableConstants.Customers.UpdatedAt})
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

        await _connection.ExecuteAsync(sql, new
        {
            Id = entity.Id.ToString(),
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            DefaultStreet = entity.DefaultAddress?.Street,
            DefaultCity = entity.DefaultAddress?.City,
            DefaultState = entity.DefaultAddress?.State,
            DefaultZipCode = entity.DefaultAddress?.ZipCode,
            DefaultCountry = entity.DefaultAddress?.Country,
            IsActive = entity.IsActive ? 1 : 0,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("Customer {CustomerId} added", entity.Id);
    }

    public async Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            UPDATE {AS400TableConstants.CustomersTable}
            SET {AS400TableConstants.Customers.Email} = ?,
                {AS400TableConstants.Customers.PhoneNumber} = ?,
                {AS400TableConstants.Customers.DefaultStreet} = ?,
                {AS400TableConstants.Customers.DefaultCity} = ?,
                {AS400TableConstants.Customers.DefaultState} = ?,
                {AS400TableConstants.Customers.DefaultZipCode} = ?,
                {AS400TableConstants.Customers.DefaultCountry} = ?,
                {AS400TableConstants.Customers.IsActive} = ?,
                {AS400TableConstants.Customers.UpdatedAt} = ?
            WHERE {AS400TableConstants.Customers.Id} = ?";

        await _connection.ExecuteAsync(sql, new
        {
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            DefaultStreet = entity.DefaultAddress?.Street,
            DefaultCity = entity.DefaultAddress?.City,
            DefaultState = entity.DefaultAddress?.State,
            DefaultZipCode = entity.DefaultAddress?.ZipCode,
            DefaultCountry = entity.DefaultAddress?.Country,
            IsActive = entity.IsActive ? 1 : 0,
            UpdatedAt = entity.UpdatedAt,
            Id = entity.Id.ToString()
        });

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("Customer {CustomerId} updated", entity.Id);
    }

    public async Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            DELETE FROM {AS400TableConstants.CustomersTable}
            WHERE {AS400TableConstants.Customers.Id} = ?";

        await _connection.ExecuteAsync(sql, new { Id = entity.Id.ToString() });
        _logger.LogInformation("Customer {CustomerId} deleted", entity.Id);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Customers.Id}, {AS400TableConstants.Customers.FirstName},
                   {AS400TableConstants.Customers.LastName}, {AS400TableConstants.Customers.Email},
                   {AS400TableConstants.Customers.PhoneNumber}, {AS400TableConstants.Customers.DefaultStreet},
                   {AS400TableConstants.Customers.DefaultCity}, {AS400TableConstants.Customers.DefaultState},
                   {AS400TableConstants.Customers.DefaultZipCode}, {AS400TableConstants.Customers.DefaultCountry},
                   {AS400TableConstants.Customers.IsActive}, {AS400TableConstants.Customers.CreatedAt},
                   {AS400TableConstants.Customers.UpdatedAt}
            FROM {AS400TableConstants.CustomersTable}
            WHERE {AS400TableConstants.Customers.Email} = ?";

        var dto = await _connection.QueryFirstOrDefaultAsync<CustomerDto>(sql, new { Email = email });
        return dto != null ? MapToEntity(dto) : null;
    }

    private Customer MapToEntity(CustomerDto dto)
    {
        Address? defaultAddress = null;
        if (!string.IsNullOrEmpty(dto.DefaultStreet))
        {
            defaultAddress = Address.Create(dto.DefaultStreet, dto.DefaultCity, dto.DefaultState, 
                dto.DefaultZipCode, dto.DefaultCountry);
        }

        var customer = Customer.Create(dto.FirstName, dto.LastName, dto.Email, dto.PhoneNumber, defaultAddress);

        var idProperty = typeof(Customer).GetProperty("Id");
        idProperty?.SetValue(customer, Guid.Parse(dto.Id));

        var isActiveProperty = typeof(Customer).GetProperty("IsActive");
        isActiveProperty?.SetValue(customer, dto.IsActive);

        var createdAtProperty = typeof(Customer).GetProperty("CreatedAt");
        createdAtProperty?.SetValue(customer, dto.CreatedAt);

        var updatedAtProperty = typeof(Customer).GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(customer, dto.UpdatedAt);

        return customer;
    }

    private class CustomerDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string DefaultStreet { get; set; } = string.Empty;
        public string DefaultCity { get; set; } = string.Empty;
        public string DefaultState { get; set; } = string.Empty;
        public string DefaultZipCode { get; set; } = string.Empty;
        public string DefaultCountry { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
