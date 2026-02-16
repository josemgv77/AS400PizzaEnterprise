using AS400PizzaEnterprise.Domain.Entities;
using AS400PizzaEnterprise.Domain.Interfaces;
using AS400PizzaEnterprise.Infrastructure.AS400;
using Microsoft.Extensions.Logging;

namespace AS400PizzaEnterprise.Infrastructure.Persistence.Repositories;

public class DeliveryPersonRepository : IDeliveryPersonRepository
{
    private readonly IAS400Connection _connection;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeliveryPersonRepository> _logger;

    public DeliveryPersonRepository(IAS400Connection connection, IUnitOfWork unitOfWork, ILogger<DeliveryPersonRepository> logger)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DeliveryPerson?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.DeliveryPersons.Id}, {AS400TableConstants.DeliveryPersons.FirstName},
                   {AS400TableConstants.DeliveryPersons.LastName}, {AS400TableConstants.DeliveryPersons.PhoneNumber},
                   {AS400TableConstants.DeliveryPersons.VehiclePlate}, {AS400TableConstants.DeliveryPersons.IsAvailable},
                   {AS400TableConstants.DeliveryPersons.IsActive}, {AS400TableConstants.DeliveryPersons.CreatedAt},
                   {AS400TableConstants.DeliveryPersons.UpdatedAt}
            FROM {AS400TableConstants.DeliveryPersonsTable}
            WHERE {AS400TableConstants.DeliveryPersons.Id} = ?";

        var dto = await _connection.QueryFirstOrDefaultAsync<DeliveryPersonDto>(sql, new { Id = id.ToString() });
        return dto != null ? MapToEntity(dto) : null;
    }

    public async Task<IEnumerable<DeliveryPerson>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.DeliveryPersons.Id}, {AS400TableConstants.DeliveryPersons.FirstName},
                   {AS400TableConstants.DeliveryPersons.LastName}, {AS400TableConstants.DeliveryPersons.PhoneNumber},
                   {AS400TableConstants.DeliveryPersons.VehiclePlate}, {AS400TableConstants.DeliveryPersons.IsAvailable},
                   {AS400TableConstants.DeliveryPersons.IsActive}, {AS400TableConstants.DeliveryPersons.CreatedAt},
                   {AS400TableConstants.DeliveryPersons.UpdatedAt}
            FROM {AS400TableConstants.DeliveryPersonsTable}
            ORDER BY {AS400TableConstants.DeliveryPersons.LastName}, {AS400TableConstants.DeliveryPersons.FirstName}";

        var dtos = await _connection.QueryAsync<DeliveryPersonDto>(sql);
        return dtos.Select(MapToEntity);
    }

    public async Task AddAsync(DeliveryPerson entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            INSERT INTO {AS400TableConstants.DeliveryPersonsTable}
            ({AS400TableConstants.DeliveryPersons.Id}, {AS400TableConstants.DeliveryPersons.FirstName},
             {AS400TableConstants.DeliveryPersons.LastName}, {AS400TableConstants.DeliveryPersons.PhoneNumber},
             {AS400TableConstants.DeliveryPersons.VehiclePlate}, {AS400TableConstants.DeliveryPersons.IsAvailable},
             {AS400TableConstants.DeliveryPersons.IsActive}, {AS400TableConstants.DeliveryPersons.CreatedAt},
             {AS400TableConstants.DeliveryPersons.UpdatedAt})
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

        await _connection.ExecuteAsync(sql, new
        {
            Id = entity.Id.ToString(),
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            PhoneNumber = entity.PhoneNumber,
            VehiclePlate = entity.VehiclePlate,
            IsAvailable = entity.IsAvailable ? 1 : 0,
            IsActive = entity.IsActive ? 1 : 0,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("DeliveryPerson {DeliveryPersonId} added", entity.Id);
    }

    public async Task UpdateAsync(DeliveryPerson entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            UPDATE {AS400TableConstants.DeliveryPersonsTable}
            SET {AS400TableConstants.DeliveryPersons.IsAvailable} = ?,
                {AS400TableConstants.DeliveryPersons.IsActive} = ?,
                {AS400TableConstants.DeliveryPersons.UpdatedAt} = ?
            WHERE {AS400TableConstants.DeliveryPersons.Id} = ?";

        await _connection.ExecuteAsync(sql, new
        {
            IsAvailable = entity.IsAvailable ? 1 : 0,
            IsActive = entity.IsActive ? 1 : 0,
            UpdatedAt = entity.UpdatedAt,
            Id = entity.Id.ToString()
        });

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("DeliveryPerson {DeliveryPersonId} updated", entity.Id);
    }

    public async Task DeleteAsync(DeliveryPerson entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            DELETE FROM {AS400TableConstants.DeliveryPersonsTable}
            WHERE {AS400TableConstants.DeliveryPersons.Id} = ?";

        await _connection.ExecuteAsync(sql, new { Id = entity.Id.ToString() });
        _logger.LogInformation("DeliveryPerson {DeliveryPersonId} deleted", entity.Id);
    }

    public async Task<IEnumerable<DeliveryPerson>> GetAvailableDeliveryPersonsAsync(CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.DeliveryPersons.Id}, {AS400TableConstants.DeliveryPersons.FirstName},
                   {AS400TableConstants.DeliveryPersons.LastName}, {AS400TableConstants.DeliveryPersons.PhoneNumber},
                   {AS400TableConstants.DeliveryPersons.VehiclePlate}, {AS400TableConstants.DeliveryPersons.IsAvailable},
                   {AS400TableConstants.DeliveryPersons.IsActive}, {AS400TableConstants.DeliveryPersons.CreatedAt},
                   {AS400TableConstants.DeliveryPersons.UpdatedAt}
            FROM {AS400TableConstants.DeliveryPersonsTable}
            WHERE {AS400TableConstants.DeliveryPersons.IsAvailable} = ? 
              AND {AS400TableConstants.DeliveryPersons.IsActive} = ?
            ORDER BY {AS400TableConstants.DeliveryPersons.LastName}, {AS400TableConstants.DeliveryPersons.FirstName}";

        var dtos = await _connection.QueryAsync<DeliveryPersonDto>(sql, new { IsAvailable = 1, IsActive = 1 });
        return dtos.Select(MapToEntity);
    }

    private DeliveryPerson MapToEntity(DeliveryPersonDto dto)
    {
        var deliveryPerson = DeliveryPerson.Create(dto.FirstName, dto.LastName, dto.PhoneNumber, dto.VehiclePlate);

        var idProperty = typeof(DeliveryPerson).GetProperty("Id");
        idProperty?.SetValue(deliveryPerson, Guid.Parse(dto.Id));

        var isAvailableProperty = typeof(DeliveryPerson).GetProperty("IsAvailable");
        isAvailableProperty?.SetValue(deliveryPerson, dto.IsAvailable);

        var isActiveProperty = typeof(DeliveryPerson).GetProperty("IsActive");
        isActiveProperty?.SetValue(deliveryPerson, dto.IsActive);

        var createdAtProperty = typeof(DeliveryPerson).GetProperty("CreatedAt");
        createdAtProperty?.SetValue(deliveryPerson, dto.CreatedAt);

        var updatedAtProperty = typeof(DeliveryPerson).GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(deliveryPerson, dto.UpdatedAt);

        return deliveryPerson;
    }

    private class DeliveryPersonDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
