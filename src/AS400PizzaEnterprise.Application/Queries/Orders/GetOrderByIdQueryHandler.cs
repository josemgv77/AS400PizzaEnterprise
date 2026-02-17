using AS400PizzaEnterprise.Application.DTOs;
using AS400PizzaEnterprise.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AS400PizzaEnterprise.Application.Queries.Orders;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithItemsAsync(request.Id, cancellationToken);
        
        if (order == null)
        {
            return null;
        }

        return _mapper.Map<OrderDto>(order);
    }
}
