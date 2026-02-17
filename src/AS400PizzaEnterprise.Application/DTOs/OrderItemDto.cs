namespace AS400PizzaEnterprise.Application.DTOs;

public record OrderItemDto(
    Guid Id,
    string PizzaName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);
