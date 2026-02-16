using AS400PizzaEnterprise.Application.DTOs;
using MediatR;

namespace AS400PizzaEnterprise.Application.Queries.Orders;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;
