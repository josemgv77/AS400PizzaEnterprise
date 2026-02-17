using AS400PizzaEnterprise.Domain.Common;
using AS400PizzaEnterprise.Domain.ValueObjects;

namespace AS400PizzaEnterprise.Domain.Events;

public class PaymentCompletedEvent : DomainEvent
{
    public Guid PaymentId { get; private set; }
    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; }

    public PaymentCompletedEvent(Guid paymentId, Guid orderId, Money amount)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Amount = amount;
    }
}
