using AS400PizzaEnterprise.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AS400PizzaEnterprise.Application.EventHandlers;

public class OrderConfirmedEventHandler : INotificationHandler<OrderConfirmedEvent>
{
    private readonly ILogger<OrderConfirmedEventHandler> _logger;

    public OrderConfirmedEventHandler(ILogger<OrderConfirmedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order confirmed: {OrderId}", notification.OrderId);
        return Task.CompletedTask;
    }
}
