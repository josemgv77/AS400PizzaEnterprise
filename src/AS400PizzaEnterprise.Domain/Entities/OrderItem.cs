using AS400PizzaEnterprise.Domain.Common;
using AS400PizzaEnterprise.Domain.ValueObjects;
using AS400PizzaEnterprise.Domain.Exceptions;

namespace AS400PizzaEnterprise.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid PizzaId { get; private set; }
    public string PizzaName { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money Subtotal { get; private set; }

    private OrderItem()
    {
        PizzaName = string.Empty;
        UnitPrice = Money.Zero;
        Subtotal = Money.Zero;
    }

    public static OrderItem Create(Guid orderId, Guid pizzaId, string pizzaName, int quantity, Money unitPrice)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("Order ID cannot be empty");

        if (pizzaId == Guid.Empty)
            throw new DomainException("Pizza ID cannot be empty");

        if (string.IsNullOrWhiteSpace(pizzaName))
            throw new DomainException("Pizza name cannot be empty");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        if (unitPrice == null)
            throw new DomainException("Unit price cannot be null");

        var orderItem = new OrderItem
        {
            OrderId = orderId,
            PizzaId = pizzaId,
            PizzaName = pizzaName,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Subtotal = unitPrice.Multiply(quantity)
        };

        return orderItem;
    }
}
