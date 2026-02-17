namespace AS400PizzaEnterprise.Application.DTOs;

public record CreateOrderItemDto(
    Guid PizzaId,
    int Quantity
);
