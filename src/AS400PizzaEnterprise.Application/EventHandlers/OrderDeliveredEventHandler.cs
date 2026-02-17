using AS400PizzaEnterprise.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AS400PizzaEnterprise.Application.EventHandlers;

public class OrderDeliveredEventHandler : INotificationHandler<OrderDeliveredEvent>
{
    private readonly ILogger<OrderDeliveredEventHandler> _logger;

    public OrderDeliveredEventHandler(ILogger<OrderDeliveredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderDeliveredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order delivered: {OrderId} by {DeliveryPersonId}", 
            notification.OrderId, 
            notification.DeliveryPersonId);
        return Task.CompletedTask;
    }
}
