using AS400PizzaEnterprise.Domain.Common;
using AS400PizzaEnterprise.Domain.Exceptions;

namespace AS400PizzaEnterprise.Domain.Entities;

public class DeliveryPerson : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string VehiclePlate { get; private set; }
    public bool IsAvailable { get; private set; }
    public bool IsActive { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private DeliveryPerson()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        PhoneNumber = string.Empty;
        VehiclePlate = string.Empty;
    }

    public static DeliveryPerson Create(string firstName, string lastName, string phoneNumber, string vehiclePlate)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number cannot be empty");

        if (string.IsNullOrWhiteSpace(vehiclePlate))
            throw new DomainException("Vehicle plate cannot be empty");

        var deliveryPerson = new DeliveryPerson
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            VehiclePlate = vehiclePlate,
            IsAvailable = true,
            IsActive = true
        };

        return deliveryPerson;
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        IsAvailable = false;
        UpdateTimestamp();
    }
}
