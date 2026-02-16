using AS400PizzaEnterprise.Domain.Entities;

namespace AS400PizzaEnterprise.Domain.Interfaces;

public interface IPizzaRepository : IRepository<Pizza>
{
    Task<IEnumerable<Pizza>> GetAvailablePizzasAsync(CancellationToken cancellationToken = default);
}
