using AS400PizzaEnterprise.Domain.Entities;
using AS400PizzaEnterprise.Domain.Enums;
using AS400PizzaEnterprise.Domain.Interfaces;
using AS400PizzaEnterprise.Domain.ValueObjects;
using AS400PizzaEnterprise.Infrastructure.AS400;
using Microsoft.Extensions.Logging;

namespace AS400PizzaEnterprise.Infrastructure.Persistence.Repositories;

public class PizzaRepository : IPizzaRepository
{
    private readonly IAS400Connection _connection;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PizzaRepository> _logger;

    public PizzaRepository(IAS400Connection connection, IUnitOfWork unitOfWork, ILogger<PizzaRepository> logger)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Pizza?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Pizzas.Id}, {AS400TableConstants.Pizzas.Name},
                   {AS400TableConstants.Pizzas.Description}, {AS400TableConstants.Pizzas.BasePrice},
                   {AS400TableConstants.Pizzas.Currency}, {AS400TableConstants.Pizzas.Size},
                   {AS400TableConstants.Pizzas.IsAvailable}, {AS400TableConstants.Pizzas.CreatedAt},
                   {AS400TableConstants.Pizzas.UpdatedAt}
            FROM {AS400TableConstants.PizzasTable}
            WHERE {AS400TableConstants.Pizzas.Id} = ?";

        var dto = await _connection.QueryFirstOrDefaultAsync<PizzaDto>(sql, new { Id = id.ToString() });
        return dto != null ? MapToEntity(dto) : null;
    }

    public async Task<IEnumerable<Pizza>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Pizzas.Id}, {AS400TableConstants.Pizzas.Name},
                   {AS400TableConstants.Pizzas.Description}, {AS400TableConstants.Pizzas.BasePrice},
                   {AS400TableConstants.Pizzas.Currency}, {AS400TableConstants.Pizzas.Size},
                   {AS400TableConstants.Pizzas.IsAvailable}, {AS400TableConstants.Pizzas.CreatedAt},
                   {AS400TableConstants.Pizzas.UpdatedAt}
            FROM {AS400TableConstants.PizzasTable}
            ORDER BY {AS400TableConstants.Pizzas.Name}";

        var dtos = await _connection.QueryAsync<PizzaDto>(sql);
        return dtos.Select(MapToEntity);
    }

    public async Task AddAsync(Pizza entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            INSERT INTO {AS400TableConstants.PizzasTable}
            ({AS400TableConstants.Pizzas.Id}, {AS400TableConstants.Pizzas.Name},
             {AS400TableConstants.Pizzas.Description}, {AS400TableConstants.Pizzas.BasePrice},
             {AS400TableConstants.Pizzas.Currency}, {AS400TableConstants.Pizzas.Size},
             {AS400TableConstants.Pizzas.IsAvailable}, {AS400TableConstants.Pizzas.CreatedAt},
             {AS400TableConstants.Pizzas.UpdatedAt})
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

        await _connection.ExecuteAsync(sql, new
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            BasePrice = entity.BasePrice.Amount,
            Currency = entity.BasePrice.Currency,
            Size = entity.Size.ToString(),
            IsAvailable = entity.IsAvailable ? 1 : 0,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("Pizza {PizzaId} added", entity.Id);
    }

    public async Task UpdateAsync(Pizza entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            UPDATE {AS400TableConstants.PizzasTable}
            SET {AS400TableConstants.Pizzas.BasePrice} = ?,
                {AS400TableConstants.Pizzas.Currency} = ?,
                {AS400TableConstants.Pizzas.IsAvailable} = ?,
                {AS400TableConstants.Pizzas.UpdatedAt} = ?
            WHERE {AS400TableConstants.Pizzas.Id} = ?";

        await _connection.ExecuteAsync(sql, new
        {
            BasePrice = entity.BasePrice.Amount,
            Currency = entity.BasePrice.Currency,
            IsAvailable = entity.IsAvailable ? 1 : 0,
            UpdatedAt = entity.UpdatedAt,
            Id = entity.Id.ToString()
        });

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("Pizza {PizzaId} updated", entity.Id);
    }

    public async Task DeleteAsync(Pizza entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            DELETE FROM {AS400TableConstants.PizzasTable}
            WHERE {AS400TableConstants.Pizzas.Id} = ?";

        await _connection.ExecuteAsync(sql, new { Id = entity.Id.ToString() });
        _logger.LogInformation("Pizza {PizzaId} deleted", entity.Id);
    }

    public async Task<IEnumerable<Pizza>> GetAvailablePizzasAsync(CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Pizzas.Id}, {AS400TableConstants.Pizzas.Name},
                   {AS400TableConstants.Pizzas.Description}, {AS400TableConstants.Pizzas.BasePrice},
                   {AS400TableConstants.Pizzas.Currency}, {AS400TableConstants.Pizzas.Size},
                   {AS400TableConstants.Pizzas.IsAvailable}, {AS400TableConstants.Pizzas.CreatedAt},
                   {AS400TableConstants.Pizzas.UpdatedAt}
            FROM {AS400TableConstants.PizzasTable}
            WHERE {AS400TableConstants.Pizzas.IsAvailable} = ?
            ORDER BY {AS400TableConstants.Pizzas.Name}";

        var dtos = await _connection.QueryAsync<PizzaDto>(sql, new { IsAvailable = 1 });
        return dtos.Select(MapToEntity);
    }

    private Pizza MapToEntity(PizzaDto dto)
    {
        var basePrice = Money.Create(dto.BasePrice, dto.Currency);
        var size = Enum.Parse<PizzaSize>(dto.Size);
        var pizza = Pizza.Create(dto.Name, dto.Description, basePrice, size, dto.IsAvailable);

        var idProperty = typeof(Pizza).GetProperty("Id");
        idProperty?.SetValue(pizza, Guid.Parse(dto.Id));

        var createdAtProperty = typeof(Pizza).GetProperty("CreatedAt");
        createdAtProperty?.SetValue(pizza, dto.CreatedAt);

        var updatedAtProperty = typeof(Pizza).GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(pizza, dto.UpdatedAt);

        return pizza;
    }

    private class PizzaDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
