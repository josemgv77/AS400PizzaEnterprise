using AS400PizzaEnterprise.Domain.Entities;
using AS400PizzaEnterprise.Domain.Enums;
using AS400PizzaEnterprise.Domain.Interfaces;
using AS400PizzaEnterprise.Domain.ValueObjects;
using AS400PizzaEnterprise.Infrastructure.AS400;
using Microsoft.Extensions.Logging;

namespace AS400PizzaEnterprise.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IAS400Connection _connection;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(IAS400Connection connection, IUnitOfWork unitOfWork, ILogger<OrderRepository> logger)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetWithItemsAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Orders.Id}, {AS400TableConstants.Orders.OrderNumber}, 
                   {AS400TableConstants.Orders.CustomerId}, {AS400TableConstants.Orders.OrderDate},
                   {AS400TableConstants.Orders.Status}, {AS400TableConstants.Orders.TotalAmount},
                   {AS400TableConstants.Orders.Currency}, {AS400TableConstants.Orders.DeliveryStreet},
                   {AS400TableConstants.Orders.DeliveryCity}, {AS400TableConstants.Orders.DeliveryState},
                   {AS400TableConstants.Orders.DeliveryZipCode}, {AS400TableConstants.Orders.DeliveryCountry},
                   {AS400TableConstants.Orders.DeliveryPersonId}, {AS400TableConstants.Orders.CreatedAt},
                   {AS400TableConstants.Orders.UpdatedAt}
            FROM {AS400TableConstants.OrdersTable}
            ORDER BY {AS400TableConstants.Orders.OrderDate} DESC";

        var orderDtos = await _connection.QueryAsync<OrderDto>(sql);
        return orderDtos.Select(MapToEntity);
    }

    public async Task AddAsync(Order entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            INSERT INTO {AS400TableConstants.OrdersTable} 
            ({AS400TableConstants.Orders.Id}, {AS400TableConstants.Orders.OrderNumber}, 
             {AS400TableConstants.Orders.CustomerId}, {AS400TableConstants.Orders.OrderDate},
             {AS400TableConstants.Orders.Status}, {AS400TableConstants.Orders.TotalAmount},
             {AS400TableConstants.Orders.Currency}, {AS400TableConstants.Orders.DeliveryStreet},
             {AS400TableConstants.Orders.DeliveryCity}, {AS400TableConstants.Orders.DeliveryState},
             {AS400TableConstants.Orders.DeliveryZipCode}, {AS400TableConstants.Orders.DeliveryCountry},
             {AS400TableConstants.Orders.DeliveryPersonId}, {AS400TableConstants.Orders.CreatedAt},
             {AS400TableConstants.Orders.UpdatedAt})
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

        await _connection.ExecuteAsync(sql, new
        {
            Id = entity.Id.ToString(),
            OrderNumber = entity.OrderNumber,
            CustomerId = entity.CustomerId.ToString(),
            OrderDate = entity.OrderDate,
            Status = entity.Status.ToString(),
            TotalAmount = entity.TotalAmount.Amount,
            Currency = entity.TotalAmount.Currency,
            DeliveryStreet = entity.DeliveryAddress.Street,
            DeliveryCity = entity.DeliveryAddress.City,
            DeliveryState = entity.DeliveryAddress.State,
            DeliveryZipCode = entity.DeliveryAddress.ZipCode,
            DeliveryCountry = entity.DeliveryAddress.Country,
            DeliveryPersonId = entity.DeliveryPersonId?.ToString(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });

        foreach (var item in entity.Items)
        {
            var itemSql = $@"
                INSERT INTO {AS400TableConstants.OrderItemsTable}
                ({AS400TableConstants.OrderItems.Id}, {AS400TableConstants.OrderItems.OrderId},
                 {AS400TableConstants.OrderItems.PizzaId}, {AS400TableConstants.OrderItems.PizzaName},
                 {AS400TableConstants.OrderItems.Quantity}, {AS400TableConstants.OrderItems.UnitPrice},
                 {AS400TableConstants.OrderItems.Currency}, {AS400TableConstants.OrderItems.Subtotal},
                 {AS400TableConstants.OrderItems.CreatedAt}, {AS400TableConstants.OrderItems.UpdatedAt})
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            await _connection.ExecuteAsync(itemSql, new
            {
                Id = item.Id.ToString(),
                OrderId = item.OrderId.ToString(),
                PizzaId = item.PizzaId.ToString(),
                PizzaName = item.PizzaName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.Amount,
                Currency = item.UnitPrice.Currency,
                Subtotal = item.Subtotal.Amount,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            });
        }

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("Order {OrderId} added with {ItemCount} items", entity.Id, entity.Items.Count);
    }

    public async Task UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            UPDATE {AS400TableConstants.OrdersTable}
            SET {AS400TableConstants.Orders.Status} = ?,
                {AS400TableConstants.Orders.TotalAmount} = ?,
                {AS400TableConstants.Orders.DeliveryPersonId} = ?,
                {AS400TableConstants.Orders.UpdatedAt} = ?
            WHERE {AS400TableConstants.Orders.Id} = ?";

        await _connection.ExecuteAsync(sql, new
        {
            Status = entity.Status.ToString(),
            TotalAmount = entity.TotalAmount.Amount,
            DeliveryPersonId = entity.DeliveryPersonId?.ToString(),
            UpdatedAt = entity.UpdatedAt,
            Id = entity.Id.ToString()
        });

        _unitOfWork.TrackEntity(entity);
        _logger.LogInformation("Order {OrderId} updated", entity.Id);
    }

    public async Task DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        var deleteItemsSql = $@"
            DELETE FROM {AS400TableConstants.OrderItemsTable}
            WHERE {AS400TableConstants.OrderItems.OrderId} = ?";

        await _connection.ExecuteAsync(deleteItemsSql, new { OrderId = entity.Id.ToString() });

        var deleteOrderSql = $@"
            DELETE FROM {AS400TableConstants.OrdersTable}
            WHERE {AS400TableConstants.Orders.Id} = ?";

        await _connection.ExecuteAsync(deleteOrderSql, new { Id = entity.Id.ToString() });

        _logger.LogInformation("Order {OrderId} deleted", entity.Id);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Orders.Id}, {AS400TableConstants.Orders.OrderNumber}, 
                   {AS400TableConstants.Orders.CustomerId}, {AS400TableConstants.Orders.OrderDate},
                   {AS400TableConstants.Orders.Status}, {AS400TableConstants.Orders.TotalAmount},
                   {AS400TableConstants.Orders.Currency}, {AS400TableConstants.Orders.DeliveryStreet},
                   {AS400TableConstants.Orders.DeliveryCity}, {AS400TableConstants.Orders.DeliveryState},
                   {AS400TableConstants.Orders.DeliveryZipCode}, {AS400TableConstants.Orders.DeliveryCountry},
                   {AS400TableConstants.Orders.DeliveryPersonId}, {AS400TableConstants.Orders.CreatedAt},
                   {AS400TableConstants.Orders.UpdatedAt}
            FROM {AS400TableConstants.OrdersTable}
            WHERE {AS400TableConstants.Orders.OrderNumber} = ?";

        var orderDto = await _connection.QueryFirstOrDefaultAsync<OrderDto>(sql, new { OrderNumber = orderNumber });
        
        if (orderDto == null)
            return null;

        return await LoadOrderWithItemsAsync(orderDto, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Orders.Id}, {AS400TableConstants.Orders.OrderNumber}, 
                   {AS400TableConstants.Orders.CustomerId}, {AS400TableConstants.Orders.OrderDate},
                   {AS400TableConstants.Orders.Status}, {AS400TableConstants.Orders.TotalAmount},
                   {AS400TableConstants.Orders.Currency}, {AS400TableConstants.Orders.DeliveryStreet},
                   {AS400TableConstants.Orders.DeliveryCity}, {AS400TableConstants.Orders.DeliveryState},
                   {AS400TableConstants.Orders.DeliveryZipCode}, {AS400TableConstants.Orders.DeliveryCountry},
                   {AS400TableConstants.Orders.DeliveryPersonId}, {AS400TableConstants.Orders.CreatedAt},
                   {AS400TableConstants.Orders.UpdatedAt}
            FROM {AS400TableConstants.OrdersTable}
            WHERE {AS400TableConstants.Orders.CustomerId} = ?
            ORDER BY {AS400TableConstants.Orders.OrderDate} DESC";

        var orderDtos = await _connection.QueryAsync<OrderDto>(sql, new { CustomerId = customerId.ToString() });
        return orderDtos.Select(MapToEntity);
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Orders.Id}, {AS400TableConstants.Orders.OrderNumber}, 
                   {AS400TableConstants.Orders.CustomerId}, {AS400TableConstants.Orders.OrderDate},
                   {AS400TableConstants.Orders.Status}, {AS400TableConstants.Orders.TotalAmount},
                   {AS400TableConstants.Orders.Currency}, {AS400TableConstants.Orders.DeliveryStreet},
                   {AS400TableConstants.Orders.DeliveryCity}, {AS400TableConstants.Orders.DeliveryState},
                   {AS400TableConstants.Orders.DeliveryZipCode}, {AS400TableConstants.Orders.DeliveryCountry},
                   {AS400TableConstants.Orders.DeliveryPersonId}, {AS400TableConstants.Orders.CreatedAt},
                   {AS400TableConstants.Orders.UpdatedAt}
            FROM {AS400TableConstants.OrdersTable}
            WHERE {AS400TableConstants.Orders.Status} = ?
            ORDER BY {AS400TableConstants.Orders.OrderDate} DESC";

        var orderDtos = await _connection.QueryAsync<OrderDto>(sql, new { Status = status.ToString() });
        return orderDtos.Select(MapToEntity);
    }

    public async Task<IEnumerable<Order>> GetPendingDeliveryOrdersAsync(CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Orders.Id}, {AS400TableConstants.Orders.OrderNumber}, 
                   {AS400TableConstants.Orders.CustomerId}, {AS400TableConstants.Orders.OrderDate},
                   {AS400TableConstants.Orders.Status}, {AS400TableConstants.Orders.TotalAmount},
                   {AS400TableConstants.Orders.Currency}, {AS400TableConstants.Orders.DeliveryStreet},
                   {AS400TableConstants.Orders.DeliveryCity}, {AS400TableConstants.Orders.DeliveryState},
                   {AS400TableConstants.Orders.DeliveryZipCode}, {AS400TableConstants.Orders.DeliveryCountry},
                   {AS400TableConstants.Orders.DeliveryPersonId}, {AS400TableConstants.Orders.CreatedAt},
                   {AS400TableConstants.Orders.UpdatedAt}
            FROM {AS400TableConstants.OrdersTable}
            WHERE {AS400TableConstants.Orders.Status} IN (?, ?, ?)
            ORDER BY {AS400TableConstants.Orders.OrderDate} ASC";

        var orderDtos = await _connection.QueryAsync<OrderDto>(sql, new 
        { 
            Status1 = OrderStatus.Confirmed.ToString(),
            Status2 = OrderStatus.InPreparation.ToString(),
            Status3 = OrderStatus.ReadyForDelivery.ToString()
        });
        
        return orderDtos.Select(MapToEntity);
    }

    public async Task<Order?> GetWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var sql = $@"
            SELECT {AS400TableConstants.Orders.Id}, {AS400TableConstants.Orders.OrderNumber}, 
                   {AS400TableConstants.Orders.CustomerId}, {AS400TableConstants.Orders.OrderDate},
                   {AS400TableConstants.Orders.Status}, {AS400TableConstants.Orders.TotalAmount},
                   {AS400TableConstants.Orders.Currency}, {AS400TableConstants.Orders.DeliveryStreet},
                   {AS400TableConstants.Orders.DeliveryCity}, {AS400TableConstants.Orders.DeliveryState},
                   {AS400TableConstants.Orders.DeliveryZipCode}, {AS400TableConstants.Orders.DeliveryCountry},
                   {AS400TableConstants.Orders.DeliveryPersonId}, {AS400TableConstants.Orders.CreatedAt},
                   {AS400TableConstants.Orders.UpdatedAt}
            FROM {AS400TableConstants.OrdersTable}
            WHERE {AS400TableConstants.Orders.Id} = ?";

        var orderDto = await _connection.QueryFirstOrDefaultAsync<OrderDto>(sql, new { Id = orderId.ToString() });
        
        if (orderDto == null)
            return null;

        return await LoadOrderWithItemsAsync(orderDto, cancellationToken);
    }

    private async Task<Order> LoadOrderWithItemsAsync(OrderDto orderDto, CancellationToken cancellationToken)
    {
        var itemsSql = $@"
            SELECT {AS400TableConstants.OrderItems.Id}, {AS400TableConstants.OrderItems.OrderId},
                   {AS400TableConstants.OrderItems.PizzaId}, {AS400TableConstants.OrderItems.PizzaName},
                   {AS400TableConstants.OrderItems.Quantity}, {AS400TableConstants.OrderItems.UnitPrice},
                   {AS400TableConstants.OrderItems.Currency}, {AS400TableConstants.OrderItems.Subtotal},
                   {AS400TableConstants.OrderItems.CreatedAt}, {AS400TableConstants.OrderItems.UpdatedAt}
            FROM {AS400TableConstants.OrderItemsTable}
            WHERE {AS400TableConstants.OrderItems.OrderId} = ?";

        var itemDtos = await _connection.QueryAsync<OrderItemDto>(itemsSql, new { OrderId = orderDto.Id });

        var order = MapToEntity(orderDto);
        
        foreach (var itemDto in itemDtos)
        {
            var unitPrice = Money.Create(itemDto.UnitPrice, itemDto.Currency);
            order.AddItem(Guid.Parse(itemDto.PizzaId), itemDto.PizzaName, itemDto.Quantity, unitPrice);
        }

        return order;
    }

    private Order MapToEntity(OrderDto dto)
    {
        var address = Address.Create(dto.DeliveryStreet, dto.DeliveryCity, dto.DeliveryState, 
            dto.DeliveryZipCode, dto.DeliveryCountry);
        var order = Order.Create(Guid.Parse(dto.CustomerId), address);

        var idProperty = typeof(Order).GetProperty("Id");
        idProperty?.SetValue(order, Guid.Parse(dto.Id));

        var orderNumberProperty = typeof(Order).GetProperty("OrderNumber");
        orderNumberProperty?.SetValue(order, dto.OrderNumber);

        var orderDateProperty = typeof(Order).GetProperty("OrderDate");
        orderDateProperty?.SetValue(order, dto.OrderDate);

        var statusProperty = typeof(Order).GetProperty("Status");
        statusProperty?.SetValue(order, Enum.Parse<OrderStatus>(dto.Status));

        var totalAmountProperty = typeof(Order).GetProperty("TotalAmount");
        totalAmountProperty?.SetValue(order, Money.Create(dto.TotalAmount, dto.Currency));

        if (!string.IsNullOrEmpty(dto.DeliveryPersonId))
        {
            var deliveryPersonIdProperty = typeof(Order).GetProperty("DeliveryPersonId");
            deliveryPersonIdProperty?.SetValue(order, Guid.Parse(dto.DeliveryPersonId));
        }

        var createdAtField = typeof(Order).GetProperty("CreatedAt");
        createdAtField?.SetValue(order, dto.CreatedAt);

        var updatedAtField = typeof(Order).GetProperty("UpdatedAt");
        updatedAtField?.SetValue(order, dto.UpdatedAt);

        return order;
    }

    private class OrderDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string DeliveryStreet { get; set; } = string.Empty;
        public string DeliveryCity { get; set; } = string.Empty;
        public string DeliveryState { get; set; } = string.Empty;
        public string DeliveryZipCode { get; set; } = string.Empty;
        public string DeliveryCountry { get; set; } = string.Empty;
        public string? DeliveryPersonId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    private class OrderItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string PizzaId { get; set; } = string.Empty;
        public string PizzaName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
