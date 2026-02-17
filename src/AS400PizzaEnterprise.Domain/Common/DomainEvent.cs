namespace AS400PizzaEnterprise.Domain.Common;

public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; private set; }
    public DateTime OccurredOn { get; private set; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}
