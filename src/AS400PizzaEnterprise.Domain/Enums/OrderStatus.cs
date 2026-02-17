namespace AS400PizzaEnterprise.Domain.Enums;

public enum OrderStatus
{
    Pending,
    Confirmed,
    InPreparation,
    ReadyForDelivery,
    InDelivery,
    Delivered,
    Cancelled
}
