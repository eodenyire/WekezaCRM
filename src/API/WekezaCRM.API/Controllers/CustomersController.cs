using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerRepository customerRepository, 
        ILogger<CustomersController> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
    {
        try
        {
            var customers = await _customerRepository.GetAllAsync();
            var customerDtos = customers.Select(MapToDto);
            return Ok(customerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(Guid id)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound($"Customer with ID {id} not found");

            return Ok(MapToDto(customer));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get customer by email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerByEmail(string email)
    {
        try
        {
            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null)
                return NotFound($"Customer with email {email} not found");

            return Ok(MapToDto(customer));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer by email {Email}", email);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get customers by segment
    /// </summary>
    [HttpGet("segment/{segment}")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomersBySegment(string segment)
    {
        try
        {
            var customers = await _customerRepository.GetBySegmentAsync(segment);
            var customerDtos = customers.Select(MapToDto);
            return Ok(customerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers by segment {Segment}", segment);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                Segment = request.Segment,
                KYCStatus = KYCStatus.Pending,
                CustomerReference = $"CUS-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6]}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var createdCustomer = await _customerRepository.CreateAsync(customer);
            _logger.LogInformation("Created customer {CustomerId}", createdCustomer.Id);

            return CreatedAtAction(
                nameof(GetCustomer), 
                new { id = createdCustomer.Id }, 
                MapToDto(createdCustomer));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update customer
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound($"Customer with ID {id} not found");

            customer.FirstName = request.FirstName ?? customer.FirstName;
            customer.LastName = request.LastName ?? customer.LastName;
            customer.PhoneNumber = request.PhoneNumber ?? customer.PhoneNumber;
            customer.Address = request.Address ?? customer.Address;
            customer.City = request.City ?? customer.City;
            customer.Country = request.Country ?? customer.Country;
            customer.Segment = request.Segment ?? customer.Segment;
            customer.UpdatedAt = DateTime.UtcNow;
            customer.UpdatedBy = "System";

            var updatedCustomer = await _customerRepository.UpdateAsync(customer);
            _logger.LogInformation("Updated customer {CustomerId}", id);

            return Ok(MapToDto(updatedCustomer));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete customer
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCustomer(Guid id)
    {
        try
        {
            var result = await _customerRepository.DeleteAsync(id);
            if (!result)
                return NotFound($"Customer with ID {id} not found");

            _logger.LogInformation("Deleted customer {CustomerId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private static CustomerDto MapToDto(Customer customer) => new()
    {
        Id = customer.Id,
        FirstName = customer.FirstName,
        LastName = customer.LastName,
        Email = customer.Email,
        PhoneNumber = customer.PhoneNumber,
        DateOfBirth = customer.DateOfBirth,
        Address = customer.Address,
        City = customer.City,
        Country = customer.Country,
        Segment = customer.Segment,
        KYCStatus = customer.KYCStatus,
        CustomerReference = customer.CustomerReference,
        CreditScore = customer.CreditScore,
        LifetimeValue = customer.LifetimeValue,
        RiskScore = customer.RiskScore,
        IsActive = customer.IsActive
    };
}

public record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime? DateOfBirth,
    string? Address,
    string? City,
    string? Country,
    CustomerSegment Segment
);

public record UpdateCustomerRequest(
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? Country,
    CustomerSegment? Segment
);
