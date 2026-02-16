using AS400PizzaEnterprise.Domain.Common;
using AS400PizzaEnterprise.Domain.ValueObjects;

namespace AS400PizzaEnterprise.Domain.Events;

public class OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }

    public OrderCreatedEvent(Guid orderId, Guid customerId, Money totalAmount)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }
}
