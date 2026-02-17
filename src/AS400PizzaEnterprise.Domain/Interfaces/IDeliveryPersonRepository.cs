using AS400PizzaEnterprise.Domain.Entities;

namespace AS400PizzaEnterprise.Domain.Interfaces;

public interface IDeliveryPersonRepository : IRepository<DeliveryPerson>
{
    Task<IEnumerable<DeliveryPerson>> GetAvailableDeliveryPersonsAsync(CancellationToken cancellationToken = default);
}
