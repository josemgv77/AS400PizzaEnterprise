using AS400PizzaEnterprise.Application.DTOs;
using MediatR;

namespace AS400PizzaEnterprise.Application.Queries.Pizzas;

public record GetAvailablePizzasQuery() : IRequest<List<PizzaDto>>;
