using AS400PizzaEnterprise.Domain.Common;
using AS400PizzaEnterprise.Domain.ValueObjects;
using AS400PizzaEnterprise.Domain.Exceptions;

namespace AS400PizzaEnterprise.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public Address? DefaultAddress { get; private set; }
    public bool IsActive { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private Customer()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PhoneNumber = string.Empty;
    }

    public static Customer Create(string firstName, string lastName, string email, string phoneNumber, Address? defaultAddress = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number cannot be empty");

        var customer = new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            DefaultAddress = defaultAddress,
            IsActive = true
        };

        return customer;
    }

    public void UpdateContactInfo(string email, string phoneNumber, Address? defaultAddress = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number cannot be empty");

        Email = email;
        PhoneNumber = phoneNumber;
        DefaultAddress = defaultAddress;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }
}
