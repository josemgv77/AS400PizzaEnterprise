using AS400PizzaEnterprise.Application.Commands.Orders;
using AS400PizzaEnterprise.Application.DTOs;
using AS400PizzaEnterprise.Application.Queries.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AS400PizzaEnterprise.API.Controllers;

/// <summary>
/// Manages pizza orders
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all orders
    /// </summary>
    /// <returns>List of all orders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllOrdersQuery();
        var orders = await _mediator.Send(query, cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// Gets an order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var order = await _mediator.Send(query, cancellationToken);
        
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="command">Order creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = orderId }, orderId);
    }

    /// <summary>
    /// Confirms an order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpPost("{id}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var command = new ConfirmOrderCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
