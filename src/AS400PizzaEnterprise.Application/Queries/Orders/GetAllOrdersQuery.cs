using AS400PizzaEnterprise.Application.DTOs;
using MediatR;

namespace AS400PizzaEnterprise.Application.Queries.Orders;

public record GetAllOrdersQuery() : IRequest<List<OrderDto>>;
