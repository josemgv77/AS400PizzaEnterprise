using MediatR;

namespace AS400PizzaEnterprise.Application.Commands.Orders;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<Unit>;
