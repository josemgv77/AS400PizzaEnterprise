using AS400PizzaEnterprise.Domain.Entities;
using AS400PizzaEnterprise.Domain.Enums;
using AS400PizzaEnterprise.Domain.Interfaces;
using AS400PizzaEnterprise.Domain.ValueObjects;
using AS400PizzaEnterprise.Infrastructure.AS400;
using Microsoft.Extensions.Logging;

namespace AS400PizzaEnterprise.Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly IAS400Connection _connection;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentRepository> _logger;

    public PaymentRepository(IAS400Connection connection, IUnitOfWork unitOfWork, ILogger<PaymentRepository> logger)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Payments.Id}, {AS400TableConstants.Payments.OrderId},
                   {AS400TableConstants.Payments.Amount}, {AS400TableConstants.Payments.Currency},
                   {AS400TableConstants.Payments.Method}, {AS400TableConstants.Payments.Status},
                   {AS400TableConstants.Payments.TransactionId}, {AS400TableConstants.Payments.CompletedAt},
                   {AS400TableConstants.Payments.CreatedAt}, {AS400TableConstants.Payments.UpdatedAt}
            FROM {AS400TableConstants.PaymentsTable}
            WHERE {AS400TableConstants.Payments.Id} = ?";

        var dto = await _connection.QueryFirstOrDefaultAsync<PaymentDto>(sql, new { Id = id.ToString() });
        return dto != null ? MapToEntity(dto) : null;
    }

    public async Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Payments.Id}, {AS400TableConstants.Payments.OrderId},
                   {AS400TableConstants.Payments.Amount}, {AS400TableConstants.Payments.Currency},
                   {AS400TableConstants.Payments.Method}, {AS400TableConstants.Payments.Status},
                   {AS400TableConstants.Payments.TransactionId}, {AS400TableConstants.Payments.CompletedAt},
                   {AS400TableConstants.Payments.CreatedAt}, {AS400TableConstants.Payments.UpdatedAt}
            FROM {AS400TableConstants.PaymentsTable}
            ORDER BY {AS400TableConstants.Payments.CreatedAt} DESC";

        var dtos = await _connection.QueryAsync<PaymentDto>(sql);
        return dtos.Select(MapToEntity);
    }

    public async Task AddAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            INSERT INTO {AS400TableConstants.PaymentsTable}
            ({AS400TableConstants.Payments.Id}, {AS400TableConstants.Payments.OrderId},
             {AS400TableConstants.Payments.Amount}, {AS400TableConstants.Payments.Currency},
             {AS400TableConstants.Payments.Method}, {AS400TableConstants.Payments.Status},
             {AS400TableConstants.Payments.TransactionId}, {AS400TableConstants.Payments.CompletedAt},
             {AS400TableConstants.Payments.CreatedAt}, {AS400TableConstants.Payments.UpdatedAt})
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

        await _connection.ExecuteAsync(sql, new
        {
            Id = entity.Id.ToString(),
            OrderId = entity.OrderId.ToString(),
            Amount = entity.Amount.Amount,
            Currency = entity.Amount.Currency,
            Method = entity.Method.ToString(),
            Status = entity.Status.ToString(),
            TransactionId = entity.TransactionId,
            CompletedAt = entity.CompletedAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("Payment {PaymentId} added", entity.Id);
    }

    public async Task UpdateAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            UPDATE {AS400TableConstants.PaymentsTable}
            SET {AS400TableConstants.Payments.Status} = ?,
                {AS400TableConstants.Payments.TransactionId} = ?,
                {AS400TableConstants.Payments.CompletedAt} = ?,
                {AS400TableConstants.Payments.UpdatedAt} = ?
            WHERE {AS400TableConstants.Payments.Id} = ?";

        await _connection.ExecuteAsync(sql, new
        {
            Status = entity.Status.ToString(),
            TransactionId = entity.TransactionId,
            CompletedAt = entity.CompletedAt,
            UpdatedAt = entity.UpdatedAt,
            Id = entity.Id.ToString()
        });

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("Payment {PaymentId} updated", entity.Id);
    }

    public async Task DeleteAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            DELETE FROM {AS400TableConstants.PaymentsTable}
            WHERE {AS400TableConstants.Payments.Id} = ?";

        await _connection.ExecuteAsync(sql, new { Id = entity.Id.ToString() });
        _logger.LogInformation("Payment {PaymentId} deleted", entity.Id);
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Payments.Id}, {AS400TableConstants.Payments.OrderId},
                   {AS400TableConstants.Payments.Amount}, {AS400TableConstants.Payments.Currency},
                   {AS400TableConstants.Payments.Method}, {AS400TableConstants.Payments.Status},
                   {AS400TableConstants.Payments.TransactionId}, {AS400TableConstants.Payments.CompletedAt},
                   {AS400TableConstants.Payments.CreatedAt}, {AS400TableConstants.Payments.UpdatedAt}
            FROM {AS400TableConstants.PaymentsTable}
            WHERE {AS400TableConstants.Payments.OrderId} = ?";

        var dto = await _connection.QueryFirstOrDefaultAsync<PaymentDto>(sql, new { OrderId = orderId.ToString() });
        return dto != null ? MapToEntity(dto) : null;
    }

    private Payment MapToEntity(PaymentDto dto)
    {
        var amount = Money.Create(dto.Amount, dto.Currency);
        var method = Enum.Parse<PaymentMethod>(dto.Method);
        var payment = Payment.Create(Guid.Parse(dto.OrderId), amount, method);

        var idProperty = typeof(Payment).GetProperty("Id");
        idProperty?.SetValue(payment, Guid.Parse(dto.Id));

        var statusProperty = typeof(Payment).GetProperty("Status");
        statusProperty?.SetValue(payment, Enum.Parse<PaymentStatus>(dto.Status));

        var transactionIdProperty = typeof(Payment).GetProperty("TransactionId");
        transactionIdProperty?.SetValue(payment, dto.TransactionId);

        var completedAtProperty = typeof(Payment).GetProperty("CompletedAt");
        completedAtProperty?.SetValue(payment, dto.CompletedAt);

        var createdAtProperty = typeof(Payment).GetProperty("CreatedAt");
        createdAtProperty?.SetValue(payment, dto.CreatedAt);

        var updatedAtProperty = typeof(Payment).GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(payment, dto.UpdatedAt);

        return payment;
    }

    private class PaymentDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
