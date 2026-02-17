# AS400 Pizza Enterprise 🍕

A modern .NET 8 enterprise pizza ordering system integrated with IBM AS400/iSeries mainframe using ODBC connectivity. Built with Domain-Driven Design (DDD), Command Query Responsibility Segregation (CQRS), and clean architecture principles.

## 📋 Table of Contents

- [Technology Stack](#technology-stack)
- [Architecture Overview](#architecture-overview)
- [Prerequisites](#prerequisites)
- [ODBC Configuration](#odbc-configuration)
- [Database Setup](#database-setup)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Key Features](#key-features)
- [Development Notes](#development-notes)

## 🚀 Technology Stack

- **.NET 8** - Latest long-term support framework
- **ODBC** - Direct AS400 database connectivity (No Entity Framework Core)
- **MediatR** - CQRS pattern implementation
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation
- **Clean Architecture** - Separation of concerns with distinct layers

## 🏗️ Architecture Overview

The project follows Clean Architecture principles with four distinct layers:

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer                            │
│  (Controllers, Middleware, Configuration)               │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│               Application Layer                         │
│  (Commands, Queries, DTOs, Validators,                  │
│   Event Handlers, Behaviors)                            │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│                 Domain Layer                            │
│  (Entities, Value Objects, Domain Events,               │
│   Interfaces, Business Rules)                           │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│             Infrastructure Layer                        │
│  (AS400 ODBC Connection, Repositories,                  │
│   Unit of Work, Persistence)                            │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

- **Domain**: Core business logic, entities, value objects, domain events
- **Application**: Use cases (commands/queries), DTOs, validation, event handlers
- **Infrastructure**: External concerns (database, ODBC, repositories)
- **API**: HTTP endpoints, request/response handling, middleware

## 📦 Prerequisites

Before running the application, ensure you have:

1. **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **IBM AS400/iSeries System** with network accessibility
3. **IBM i Access ODBC Driver** - Install from IBM i Access Client Solutions
4. **Database Access Credentials** - Valid AS400 user with permissions to PIZZALIB library

### Verify .NET Installation

```bash
dotnet --version
# Should output: 8.0.x
```

## 🔌 ODBC Configuration

### Step 1: Install ODBC Driver

1. Download and install **IBM i Access Client Solutions** from IBM
2. Install the ODBC driver component during setup
3. Verify installation in ODBC Data Source Administrator (Windows) or `odbcinst -j` (Linux)

### Step 2: Create System DSN

#### Windows

1. Open **ODBC Data Source Administrator (64-bit)**
2. Go to **System DSN** tab
3. Click **Add** → Select **IBM i Access ODBC Driver**
4. Configure DSN:
   - **Data Source Name**: `MI_AS400`
   - **System**: `your.as400.hostname.com` or IP address
   - **Default Library**: `PIZZALIB`
   - Click **OK** to save

#### Linux

Create/edit `/etc/odbc.ini`:

```ini
[MI_AS400]
Description=AS400 Pizza Enterprise Database
Driver=IBM i Access ODBC Driver
System=your.as400.hostname.com
DefaultLibraries=PIZZALIB
```

### Step 3: Configure Connection String

Edit `src/AS400PizzaEnterprise.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "AS400Connection": "DSN=MI_AS400;UID=YOUR_USERNAME;PWD=YOUR_PASSWORD;"
  }
}
```

**Connection String Format:**
```
DSN=<data_source_name>;UID=<username>;PWD=<password>;
```

**Security Note**: For production, use environment variables or Azure Key Vault:

```bash
# Linux/macOS
export ConnectionStrings__AS400Connection="DSN=MI_AS400;UID=user;PWD=pass;"

# Windows
set ConnectionStrings__AS400Connection=DSN=MI_AS400;UID=user;PWD=pass;
```

## 💾 Database Setup

### Step 1: Execute SQL Script

Run the database setup script on your AS400 system:

```bash
# Location
scripts/AS400_Database_Setup.sql
```

This script will:
- Create the `PIZZALIB` library
- Create all required tables (CLIENTES, PIZZAS, PEDIDOS, etc.)
- Set up foreign key relationships
- Create necessary indexes
- Insert sample test data

### Step 2: Verify Tables

Connect to AS400 and verify tables exist:

```sql
SELECT * FROM QSYS2.SYSTABLES 
WHERE TABLE_SCHEMA = 'PIZZALIB'
```

Expected tables:
- `CLIENTES` (Customers)
- `PIZZAS` (Pizzas)
- `PEDIDOS` (Orders)
- `PEDIDOS_DET` (Order Items/Details)
- `PAGOS` (Payments)
- `REPARTIDORES` (Delivery Persons)

## 🏃 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd AS400PizzaEnterprise
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Connection String

Edit `src/AS400PizzaEnterprise.API/appsettings.json` with your AS400 credentials.

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run the API

```bash
cd src/AS400PizzaEnterprise.API
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### 6. Test the API

Open your browser or use curl:

```bash
# Get available pizzas
curl http://localhost:5000/api/pizzas/available

# Get all orders
curl http://localhost:5000/api/orders
```

## 📡 API Endpoints

### Pizzas

#### Get Available Pizzas
```http
GET /api/pizzas/available
```

**Response 200 OK:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Margherita",
    "description": "Fresh tomatoes, mozzarella, basil",
    "price": {
      "amount": 12.99,
      "currency": "USD"
    },
    "size": 2,
    "isAvailable": true
  }
]
```

### Customers

#### Get All Customers
```http
GET /api/customers
```

**Response 200 OK:**
```json
[
  {
    "id": "2fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1-555-0123",
    "fullName": "John Doe"
  }
]
```

### Orders

#### Get All Orders
```http
GET /api/orders
```

**Response 200 OK:**
```json
[
  {
    "id": "1fa85f64-5717-4562-b3fc-2c963f66afa6",
    "orderNumber": "ORD-20240101-001",
    "customerId": "2fa85f64-5717-4562-b3fc-2c963f66afa6",
    "orderDate": "2024-01-01T10:30:00Z",
    "status": 1,
    "totalAmount": {
      "amount": 25.98,
      "currency": "USD"
    },
    "items": [
      {
        "pizzaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "pizzaName": "Margherita",
        "quantity": 2,
        "unitPrice": { "amount": 12.99, "currency": "USD" }
      }
    ]
  }
]
```

#### Get Order by ID
```http
GET /api/orders/{id}
```

**Response 200 OK:** (Same as single order above)

**Response 404 Not Found:**
```json
{
  "message": "Order not found"
}
```

#### Create Order
```http
POST /api/orders
Content-Type: application/json
```

**Request Body:**
```json
{
  "customerId": "2fa85f64-5717-4562-b3fc-2c963f66afa6",
  "street": "123 Main St",
  "city": "Springfield",
  "state": "IL",
  "zipCode": "62701",
  "country": "USA",
  "items": [
    {
      "pizzaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "quantity": 2
    }
  ]
}
```

**Response 200 OK:**
```json
{
  "orderId": "1fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response 400 Bad Request:** (Validation errors)
```json
{
  "errors": {
    "CustomerId": ["Customer ID is required"],
    "Items": ["Order must contain at least one item"]
  }
}
```

#### Confirm Order
```http
POST /api/orders/{id}/confirm
```

**Response 200 OK**

**Response 404 Not Found:** Order doesn't exist

## 📂 Project Structure

```
AS400PizzaEnterprise/
├── src/
│   ├── AS400PizzaEnterprise.API/
│   │   ├── Controllers/
│   │   │   ├── OrdersController.cs
│   │   │   ├── PizzasController.cs
│   │   │   └── CustomersController.cs
│   │   ├── Middleware/
│   │   │   └── ExceptionHandlingMiddleware.cs
│   │   ├── appsettings.json
│   │   └── Program.cs
│   │
│   ├── AS400PizzaEnterprise.Application/
│   │   ├── Commands/
│   │   │   └── Orders/
│   │   │       ├── CreateOrderCommand.cs
│   │   │       ├── CreateOrderCommandHandler.cs
│   │   │       ├── ConfirmOrderCommand.cs
│   │   │       └── ConfirmOrderCommandHandler.cs
│   │   ├── Queries/
│   │   │   ├── Orders/
│   │   │   │   ├── GetAllOrdersQuery.cs
│   │   │   │   ├── GetOrderByIdQuery.cs
│   │   │   │   └── Handlers...
│   │   │   └── Pizzas/
│   │   │       ├── GetAvailablePizzasQuery.cs
│   │   │       └── Handler...
│   │   ├── DTOs/
│   │   │   ├── OrderDto.cs
│   │   │   ├── PizzaDto.cs
│   │   │   ├── CustomerDto.cs
│   │   │   └── OrderItemDto.cs
│   │   ├── Validators/
│   │   │   └── CreateOrderCommandValidator.cs
│   │   ├── EventHandlers/
│   │   │   ├── OrderCreatedEventHandler.cs
│   │   │   ├── OrderConfirmedEventHandler.cs
│   │   │   ├── OrderDeliveredEventHandler.cs
│   │   │   └── PaymentCompletedEventHandler.cs
│   │   ├── Behaviors/
│   │   │   └── ValidationBehavior.cs
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── AS400PizzaEnterprise.Domain/
│   │   ├── Entities/
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   ├── Pizza.cs
│   │   │   ├── Customer.cs
│   │   │   ├── Payment.cs
│   │   │   └── DeliveryPerson.cs
│   │   ├── ValueObjects/
│   │   │   ├── Money.cs
│   │   │   └── Address.cs
│   │   ├── Enums/
│   │   │   ├── OrderStatus.cs
│   │   │   ├── PaymentStatus.cs
│   │   │   ├── PaymentMethod.cs
│   │   │   └── PizzaSize.cs
│   │   ├── Events/
│   │   │   ├── OrderCreatedEvent.cs
│   │   │   ├── OrderConfirmedEvent.cs
│   │   │   ├── OrderDeliveredEvent.cs
│   │   │   └── PaymentCompletedEvent.cs
│   │   ├── Interfaces/
│   │   │   ├── IRepository.cs
│   │   │   ├── IOrderRepository.cs
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── IAS400Connection.cs
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs
│   │   │   └── ValueObject.cs
│   │   └── Exceptions/
│   │       └── DomainException.cs
│   │
│   └── AS400PizzaEnterprise.Infrastructure/
│       ├── AS400/
│       │   ├── AS400OdbcConnection.cs
│       │   └── AS400TableConstants.cs
│       ├── Persistence/
│       │   ├── Repositories/
│       │   │   ├── OrderRepository.cs
│       │   │   ├── PizzaRepository.cs
│       │   │   ├── CustomerRepository.cs
│       │   │   ├── PaymentRepository.cs
│       │   │   └── DeliveryPersonRepository.cs
│       │   └── UnitOfWork.cs
│       └── DependencyInjection.cs
│
├── scripts/
│   └── AS400_Database_Setup.sql
├── AS400PizzaEnterprise.slnx
└── README.md
```

## ✨ Key Features

### Domain-Driven Design (DDD)

- **Rich Domain Model**: Entities with encapsulated business logic
- **Value Objects**: Immutable objects like `Money` and `Address`
- **Domain Events**: Decoupled communication between aggregates
- **Aggregates**: Order aggregate root managing OrderItems

### CQRS Pattern

Separates read and write operations:

- **Commands**: Modify state (CreateOrder, ConfirmOrder)
- **Queries**: Read data (GetOrders, GetPizzas)
- **MediatR**: Handles command/query routing and pipeline behaviors

### Repository Pattern

Generic repository with specialized implementations:

```csharp
IRepository<Order>
IOrderRepository : IRepository<Order>  // Extended with custom queries
```

### Unit of Work Pattern

Manages transactions and domain event dispatching:

```csharp
await _unitOfWork.BeginTransaction();
await _orderRepository.AddAsync(order);
await _unitOfWork.SaveChangesAsync();  // Dispatches domain events
await _unitOfWork.CommitTransaction();
```

### Validation Pipeline

FluentValidation integrated with MediatR pipeline:

- Automatic validation before command execution
- Returns structured validation errors
- Example: `CreateOrderCommandValidator`

### ODBC Direct Connection

**Important**: This project does NOT use Entity Framework Core. It uses raw ODBC for AS400 connectivity:

- Custom `AS400OdbcConnection` class
- Generic query mapping
- Manual transaction management
- Support for AS400 data types and conventions

### Domain Events

Asynchronous event handling after aggregate changes:

- `OrderCreatedEvent` → Log order creation, send notifications
- `OrderConfirmedEvent` → Update inventory, notify kitchen
- `PaymentCompletedEvent` → Update order status, send receipt
- `OrderDeliveredEvent` → Mark complete, notify customer

### Value Objects

Immutable, behavior-rich types:

**Money:**
```csharp
var price = Money.Create(12.99m, "USD");
var total = price.Multiply(2);  // $25.98 USD
```

**Address:**
```csharp
var address = Address.Create("123 Main St", "Springfield", "IL", "62701", "USA");
Console.WriteLine(address);  // "123 Main St, Springfield, IL 62701, USA"
```

### Exception Handling

Global exception middleware:

- Catches all exceptions
- Returns structured error responses
- Logs errors with details
- Hides internal errors in production

## 🔧 Development Notes

### Database Conventions

AS400 tables use specific naming and conventions:

- **Library**: `PIZZALIB`
- **Table Names**: Spanish (CLIENTES, PEDIDOS, REPARTIDORES)
- **Column Names**: UPPER_SNAKE_CASE with underscores (ORDER_NUMBER, CUSTOMER_ID, IS_ACTIVE)
- **Boolean Fields**: `INT` with 1 (true) / 0 (false) values
- **GUIDs**: Stored as `CHAR(36)` strings
- **Decimals**: `DECIMAL(18,2)` for monetary amounts
- **Enums**: Stored as `INT` values

### Enum Values

**OrderStatus:**
- 0: Pending
- 1: Confirmed
- 2: InPreparation
- 3: ReadyForDelivery
- 4: InDelivery
- 5: Delivered
- 6: Cancelled

**PaymentMethod:**
- 0: Cash
- 1: CreditCard
- 2: DebitCard
- 3: Online

**PaymentStatus:**
- 0: Pending
- 1: Completed
- 2: Failed
- 3: Refunded

**PizzaSize:**
- 0: Small
- 1: Medium
- 2: Large
- 3: ExtraLarge

### Adding New Features

1. **Domain**: Create/modify entities and domain events
2. **Application**: Add commands/queries with handlers and validators
3. **Infrastructure**: Extend repositories if needed
4. **API**: Add controller endpoints

### Testing Connection

Test ODBC connection before running application:

```bash
# Windows
odbcad32.exe  # Test DSN connection

# Linux
isql -v MI_AS400 username password
```

### Common Issues

**Issue**: "Data source name not found"
- **Solution**: Verify DSN exists in System DSN (not User DSN)

**Issue**: "Connection timeout"
- **Solution**: Check AS400 hostname/IP, firewall rules, network connectivity

**Issue**: "Table not found"
- **Solution**: Ensure PIZZALIB library exists and user has access

**Issue**: "Invalid credentials"
- **Solution**: Verify username/password, check user profile on AS400

### Performance Tips

- Use indexes on foreign keys (CustomerId, OrderId, PizzaId)
- Limit result sets with pagination
- Use `GetWithItems` for order details (joins in query)
- Cache available pizzas on client side

## 📝 License

This project is for demonstration purposes.

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Keep domain logic in Domain layer
3. Use CQRS for new features
4. Add validators for all commands
5. Write meaningful commit messages
6. Test with actual AS400 connection

## 📞 Support

For issues or questions:
1. Check this documentation
2. Verify AS400 connectivity
3. Review application logs
4. Check domain event handlers for async issues

---

**Built with ❤️ using .NET 8 and IBM AS400/iSeries**
