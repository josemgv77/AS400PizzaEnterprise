using AS400PizzaEnterprise.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AS400PizzaEnterprise.Application.EventHandlers;

public class PaymentCompletedEventHandler : INotificationHandler<PaymentCompletedEvent>
{
    private readonly ILogger<PaymentCompletedEventHandler> _logger;

    public PaymentCompletedEventHandler(ILogger<PaymentCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payment completed: {PaymentId} for order {OrderId}", 
            notification.PaymentId, 
            notification.OrderId);
        return Task.CompletedTask;
    }
}
