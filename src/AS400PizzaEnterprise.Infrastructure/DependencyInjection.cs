using AS400PizzaEnterprise.Domain.Interfaces;
using AS400PizzaEnterprise.Infrastructure.AS400;
using AS400PizzaEnterprise.Infrastructure.Persistence;
using AS400PizzaEnterprise.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AS400PizzaEnterprise.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAS400Connection, AS400OdbcConnection>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPizzaRepository, PizzaRepository>();
        services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}
