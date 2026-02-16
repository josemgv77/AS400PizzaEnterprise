using AS400PizzaEnterprise.Domain.Common;
using AS400PizzaEnterprise.Domain.Enums;
using AS400PizzaEnterprise.Domain.ValueObjects;
using AS400PizzaEnterprise.Domain.Exceptions;

namespace AS400PizzaEnterprise.Domain.Entities;

public class Pizza : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money BasePrice { get; private set; }
    public PizzaSize Size { get; private set; }
    public bool IsAvailable { get; private set; }

    private Pizza()
    {
        Name = string.Empty;
        Description = string.Empty;
        BasePrice = Money.Zero;
    }

    public static Pizza Create(string name, string description, Money basePrice, PizzaSize size, bool isAvailable = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Pizza name cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Pizza description cannot be empty");

        if (basePrice == null)
            throw new DomainException("Base price cannot be null");

        var pizza = new Pizza
        {
            Name = name,
            Description = description,
            BasePrice = basePrice,
            Size = size,
            IsAvailable = isAvailable
        };

        return pizza;
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice == null)
            throw new DomainException("Price cannot be null");

        BasePrice = newPrice;
        UpdateTimestamp();
    }

    public void UpdateAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        UpdateTimestamp();
    }
}
