using AS400PizzaEnterprise.Domain.Common;
using AS400PizzaEnterprise.Domain.Enums;
using AS400PizzaEnterprise.Domain.ValueObjects;
using AS400PizzaEnterprise.Domain.Events;
using AS400PizzaEnterprise.Domain.Exceptions;

namespace AS400PizzaEnterprise.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private Payment()
    {
        Amount = Money.Zero;
    }

    public static Payment Create(Guid orderId, Money amount, PaymentMethod method)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("Order ID cannot be empty");

        if (amount == null)
            throw new DomainException("Amount cannot be null");

        var payment = new Payment
        {
            OrderId = orderId,
            Amount = amount,
            Method = method,
            Status = PaymentStatus.Pending
        };

        return payment;
    }

    public void Complete(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException($"Cannot complete payment in {Status} status");

        if (string.IsNullOrWhiteSpace(transactionId))
            throw new DomainException("Transaction ID cannot be empty");

        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        CompletedAt = DateTime.UtcNow;
        UpdateTimestamp();

        AddDomainEvent(new PaymentCompletedEvent(Id, OrderId, Amount));
    }

    public void Fail()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException($"Cannot fail payment in {Status} status");

        Status = PaymentStatus.Failed;
        UpdateTimestamp();
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException($"Cannot refund payment in {Status} status");

        Status = PaymentStatus.Refunded;
        UpdateTimestamp();
    }
}
