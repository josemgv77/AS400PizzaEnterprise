using AS400PizzaEnterprise.Application.DTOs;
using AS400PizzaEnterprise.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AS400PizzaEnterprise.Application.Queries.Orders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetAllOrdersQueryHandler(
        IOrderRepository orderRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<OrderDto>>(orders.ToList());
    }
}
