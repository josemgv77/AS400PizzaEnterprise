using AS400PizzaEnterprise.Application.DTOs;
using MediatR;

namespace AS400PizzaEnterprise.Application.Commands.Orders;

public record CreateOrderCommand(
    Guid CustomerId,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    List<CreateOrderItemDto> Items
) : IRequest<Guid>;
