using AS400PizzaEnterprise.Domain.Entities;
using AS400PizzaEnterprise.Domain.Interfaces;
using AS400PizzaEnterprise.Domain.ValueObjects;
using MediatR;

namespace AS400PizzaEnterprise.Application.Commands.Orders;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        ICustomerRepository customerRepository,
        IPizzaRepository pizzaRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _pizzaRepository = pizzaRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found");
        }

        var address = Address.Create(
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country
        );

        var order = Order.Create(request.CustomerId, address);

        foreach (var item in request.Items)
        {
            var pizza = await _pizzaRepository.GetByIdAsync(item.PizzaId, cancellationToken);
            if (pizza == null)
            {
                throw new InvalidOperationException($"Pizza with ID {item.PizzaId} not found");
            }

            if (!pizza.IsAvailable)
            {
                throw new InvalidOperationException($"Pizza {pizza.Name} is not available");
            }

            order.AddItem(pizza.Id, pizza.Name, item.Quantity, pizza.BasePrice);
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
