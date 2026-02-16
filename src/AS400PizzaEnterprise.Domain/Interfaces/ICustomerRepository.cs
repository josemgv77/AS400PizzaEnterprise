using AS400PizzaEnterprise.Domain.Entities;

namespace AS400PizzaEnterprise.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
