using AS400PizzaEnterprise.Domain.Common;

namespace AS400PizzaEnterprise.Domain.Events;

public class OrderConfirmedEvent : DomainEvent
{
    public Guid OrderId { get; private set; }

    public OrderConfirmedEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}
