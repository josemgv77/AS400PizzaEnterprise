using AS400PizzaEnterprise.Domain.Common;
using AS400PizzaEnterprise.Domain.Enums;
using AS400PizzaEnterprise.Domain.ValueObjects;
using AS400PizzaEnterprise.Domain.Events;
using AS400PizzaEnterprise.Domain.Exceptions;

namespace AS400PizzaEnterprise.Domain.Entities;

public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = new();

    public string OrderNumber { get; private set; }
    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public Guid? DeliveryPersonId { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order()
    {
        OrderNumber = string.Empty;
        TotalAmount = Money.Zero;
        DeliveryAddress = null!;
    }

    public static Order Create(Guid customerId, Address deliveryAddress)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("Customer ID cannot be empty");

        if (deliveryAddress == null)
            throw new DomainException("Delivery address cannot be null");

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            DeliveryAddress = deliveryAddress,
            TotalAmount = Money.Zero
        };

        order.AddDomainEvent(new OrderCreatedEvent(order.Id, customerId, order.TotalAmount));

        return order;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    public void AddItem(Guid pizzaId, string pizzaName, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot add items to order in {Status} status");

        var orderItem = OrderItem.Create(Id, pizzaId, pizzaName, quantity, unitPrice);
        _items.Add(orderItem);
        RecalculateTotal();
        UpdateTimestamp();
    }

    public void RemoveItem(Guid orderItemId)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot remove items from order in {Status} status");

        var item = _items.FirstOrDefault(i => i.Id == orderItemId);
        if (item == null)
            throw new DomainException("Order item not found");

        _items.Remove(item);
        RecalculateTotal();
        UpdateTimestamp();
    }

    private void RecalculateTotal()
    {
        if (_items.Count == 0)
        {
            TotalAmount = Money.Zero;
            return;
        }

        var total = _items.First().Subtotal;
        foreach (var item in _items.Skip(1))
        {
            total = total.Add(item.Subtotal);
        }

        TotalAmount = total;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot confirm order in {Status} status");

        if (_items.Count == 0)
            throw new DomainException("Cannot confirm order with no items");

        Status = OrderStatus.Confirmed;
        UpdateTimestamp();

        AddDomainEvent(new OrderConfirmedEvent(Id));
    }

    public void StartPreparation()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException($"Cannot start preparation for order in {Status} status");

        Status = OrderStatus.InPreparation;
        UpdateTimestamp();
    }

    public void MarkReadyForDelivery()
    {
        if (Status != OrderStatus.InPreparation)
            throw new DomainException($"Cannot mark order ready for delivery in {Status} status");

        Status = OrderStatus.ReadyForDelivery;
        UpdateTimestamp();
    }

    public void AssignDeliveryPerson(Guid deliveryPersonId)
    {
        if (deliveryPersonId == Guid.Empty)
            throw new DomainException("Delivery person ID cannot be empty");

        if (Status != OrderStatus.ReadyForDelivery)
            throw new DomainException($"Cannot assign delivery person to order in {Status} status");

        DeliveryPersonId = deliveryPersonId;
        Status = OrderStatus.InDelivery;
        UpdateTimestamp();
    }

    public void CompleteDelivery()
    {
        if (Status != OrderStatus.InDelivery)
            throw new DomainException($"Cannot complete delivery for order in {Status} status");

        if (!DeliveryPersonId.HasValue)
            throw new DomainException("Cannot complete delivery without a delivery person");

        Status = OrderStatus.Delivered;
        UpdateTimestamp();

        AddDomainEvent(new OrderDeliveredEvent(Id, DeliveryPersonId.Value));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new DomainException("Cannot cancel a delivered order");

        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Order is already cancelled");

        Status = OrderStatus.Cancelled;
        UpdateTimestamp();
    }
}
