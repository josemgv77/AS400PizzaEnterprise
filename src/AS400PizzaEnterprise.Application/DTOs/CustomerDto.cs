namespace AS400PizzaEnterprise.Application.DTOs;

public record CustomerDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string? Address
);
