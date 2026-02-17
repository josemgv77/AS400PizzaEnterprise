using AS400PizzaEnterprise.Application.DTOs;
using AS400PizzaEnterprise.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AS400PizzaEnterprise.API.Controllers;

/// <summary>
/// Manages customer information
/// </summary>
[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomersController(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all customers
    /// </summary>
    /// <returns>List of all customers</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDto>>> GetAll(CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        var customerDtos = _mapper.Map<List<CustomerDto>>(customers);
        return Ok(customerDtos);
    }
}
