using AS400PizzaEnterprise.Application.DTOs;
using AS400PizzaEnterprise.Application.Queries.Pizzas;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AS400PizzaEnterprise.API.Controllers;

/// <summary>
/// Manages pizza catalog
/// </summary>
[ApiController]
[Route("api/pizzas")]
public class PizzasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PizzasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all available pizzas
    /// </summary>
    /// <returns>List of available pizzas</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(List<PizzaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PizzaDto>>> GetAvailable(CancellationToken cancellationToken)
    {
        var query = new GetAvailablePizzasQuery();
        var pizzas = await _mediator.Send(query, cancellationToken);
        return Ok(pizzas);
    }
}
