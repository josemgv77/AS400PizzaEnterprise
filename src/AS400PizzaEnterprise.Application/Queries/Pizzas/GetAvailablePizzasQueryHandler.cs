using AS400PizzaEnterprise.Application.DTOs;
using AS400PizzaEnterprise.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AS400PizzaEnterprise.Application.Queries.Pizzas;

public class GetAvailablePizzasQueryHandler : IRequestHandler<GetAvailablePizzasQuery, List<PizzaDto>>
{
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IMapper _mapper;

    public GetAvailablePizzasQueryHandler(
        IPizzaRepository pizzaRepository,
        IMapper mapper)
    {
        _pizzaRepository = pizzaRepository;
        _mapper = mapper;
    }

    public async Task<List<PizzaDto>> Handle(GetAvailablePizzasQuery request, CancellationToken cancellationToken)
    {
        var pizzas = await _pizzaRepository.GetAvailablePizzasAsync(cancellationToken);
        return _mapper.Map<List<PizzaDto>>(pizzas.ToList());
    }
}
