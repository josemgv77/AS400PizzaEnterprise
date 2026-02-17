using AS400PizzaEnterprise.Domain.Common;

namespace AS400PizzaEnterprise.Domain.Events;

public class OrderDeliveredEvent : DomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid DeliveryPersonId { get; private set; }

    public OrderDeliveredEvent(Guid orderId, Guid deliveryPersonId)
    {
        OrderId = orderId;
        DeliveryPersonId = deliveryPersonId;
    }
}
