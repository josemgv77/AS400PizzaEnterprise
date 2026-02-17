namespace AS400PizzaEnterprise.Application.DTOs;

public record OrderDto(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    string DeliveryAddress,
    string? DeliveryPersonName,
    List<OrderItemDto> Items
);
