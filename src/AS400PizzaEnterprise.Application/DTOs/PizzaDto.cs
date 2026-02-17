namespace AS400PizzaEnterprise.Application.DTOs;

public record PizzaDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Size,
    bool IsAvailable
);
