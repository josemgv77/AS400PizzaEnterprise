namespace AS400PizzaEnterprise.Infrastructure.AS400;

public static class AS400TableConstants
{
    public const string Schema = "PIZZALIB";
    public const string OrdersTable = "PIZZALIB.PEDIDOS";
    public const string OrderItemsTable = "PIZZALIB.PEDIDOS_DET";
    public const string CustomersTable = "PIZZALIB.CLIENTES";
    public const string PizzasTable = "PIZZALIB.PIZZAS";
    public const string DeliveryPersonsTable = "PIZZALIB.REPARTIDORES";
    public const string PaymentsTable = "PIZZALIB.PAGOS";

    public static class Orders
    {
        public const string Id = "ID";
        public const string OrderNumber = "ORDER_NUMBER";
        public const string CustomerId = "CUSTOMER_ID";
        public const string OrderDate = "ORDER_DATE";
        public const string Status = "STATUS";
        public const string TotalAmount = "TOTAL_AMOUNT";
        public const string Currency = "CURRENCY";
        public const string DeliveryStreet = "DELIVERY_STREET";
        public const string DeliveryCity = "DELIVERY_CITY";
        public const string DeliveryState = "DELIVERY_STATE";
        public const string DeliveryZipCode = "DELIVERY_ZIPCODE";
        public const string DeliveryCountry = "DELIVERY_COUNTRY";
        public const string DeliveryPersonId = "DELIVERY_PERSON_ID";
        public const string CreatedAt = "CREATED_AT";
        public const string UpdatedAt = "UPDATED_AT";
    }

    public static class OrderItems
    {
        public const string Id = "ID";
        public const string OrderId = "ORDER_ID";
        public const string PizzaId = "PIZZA_ID";
        public const string PizzaName = "PIZZA_NAME";
        public const string Quantity = "QUANTITY";
        public const string UnitPrice = "UNIT_PRICE";
        public const string Currency = "CURRENCY";
        public const string Subtotal = "SUBTOTAL";
        public const string CreatedAt = "CREATED_AT";
        public const string UpdatedAt = "UPDATED_AT";
    }

    public static class Customers
    {
        public const string Id = "ID";
        public const string FirstName = "FIRST_NAME";
        public const string LastName = "LAST_NAME";
        public const string Email = "EMAIL";
        public const string PhoneNumber = "PHONE_NUMBER";
        public const string DefaultStreet = "DEFAULT_STREET";
        public const string DefaultCity = "DEFAULT_CITY";
        public const string DefaultState = "DEFAULT_STATE";
        public const string DefaultZipCode = "DEFAULT_ZIPCODE";
        public const string DefaultCountry = "DEFAULT_COUNTRY";
        public const string IsActive = "IS_ACTIVE";
        public const string CreatedAt = "CREATED_AT";
        public const string UpdatedAt = "UPDATED_AT";
    }

    public static class Pizzas
    {
        public const string Id = "ID";
        public const string Name = "NAME";
        public const string Description = "DESCRIPTION";
        public const string BasePrice = "BASE_PRICE";
        public const string Currency = "CURRENCY";
        public const string Size = "SIZE";
        public const string IsAvailable = "IS_AVAILABLE";
        public const string CreatedAt = "CREATED_AT";
        public const string UpdatedAt = "UPDATED_AT";
    }

    public static class DeliveryPersons
    {
        public const string Id = "ID";
        public const string FirstName = "FIRST_NAME";
        public const string LastName = "LAST_NAME";
        public const string PhoneNumber = "PHONE_NUMBER";
        public const string VehiclePlate = "VEHICLE_PLATE";
        public const string IsAvailable = "IS_AVAILABLE";
        public const string IsActive = "IS_ACTIVE";
        public const string CreatedAt = "CREATED_AT";
        public const string UpdatedAt = "UPDATED_AT";
    }

    public static class Payments
    {
        public const string Id = "ID";
        public const string OrderId = "ORDER_ID";
        public const string Amount = "AMOUNT";
        public const string Currency = "CURRENCY";
        public const string Method = "METHOD";
        public const string Status = "STATUS";
        public const string TransactionId = "TRANSACTION_ID";
        public const string CompletedAt = "COMPLETED_AT";
        public const string CreatedAt = "CREATED_AT";
        public const string UpdatedAt = "UPDATED_AT";
    }
}
