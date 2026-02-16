using AS400PizzaEnterprise.Domain.Entities;

namespace AS400PizzaEnterprise.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
