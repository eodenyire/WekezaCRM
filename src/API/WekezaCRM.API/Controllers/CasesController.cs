using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CasesController : ControllerBase
{
    private readonly ICaseRepository _caseRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CasesController> _logger;

    public CasesController(
        ICaseRepository caseRepository,
        ICustomerRepository customerRepository,
        ILogger<CasesController> logger)
    {
        _caseRepository = caseRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all cases
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CaseDto>>> GetAllCases()
    {
        try
        {
            var cases = await _caseRepository.GetAllAsync();
            var caseDtos = cases.Select(MapToDto);
            return Ok(caseDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cases");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get case by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CaseDto>> GetCase(Guid id)
    {
        try
        {
            var caseEntity = await _caseRepository.GetByIdAsync(id);
            if (caseEntity == null)
                return NotFound($"Case with ID {id} not found");

            return Ok(MapToDto(caseEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving case {CaseId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get cases by customer ID
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<CaseDto>>> GetCasesByCustomer(Guid customerId)
    {
        try
        {
            var cases = await _caseRepository.GetByCustomerIdAsync(customerId);
            var caseDtos = cases.Select(MapToDto);
            return Ok(caseDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cases for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new case
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CaseDto>> CreateCase([FromBody] CreateCaseRequest request)
    {
        try
        {
            // Verify customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
                return NotFound($"Customer with ID {request.CustomerId} not found");

            var caseEntity = new Case
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                CaseNumber = $"CASE-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6]}",
                Title = request.Title,
                Description = request.Description,
                Status = CaseStatus.Open,
                Priority = request.Priority,
                Category = request.Category,
                SubCategory = request.SubCategory ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var createdCase = await _caseRepository.CreateAsync(caseEntity);
            _logger.LogInformation("Created case {CaseId} for customer {CustomerId}", 
                createdCase.Id, request.CustomerId);

            return CreatedAtAction(
                nameof(GetCase), 
                new { id = createdCase.Id }, 
                MapToDto(createdCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating case");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update case status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<CaseDto>> UpdateCaseStatus(
        Guid id, 
        [FromBody] UpdateCaseStatusRequest request)
    {
        try
        {
            var caseEntity = await _caseRepository.GetByIdAsync(id);
            if (caseEntity == null)
                return NotFound($"Case with ID {id} not found");

            caseEntity.Status = request.Status;
            caseEntity.UpdatedAt = DateTime.UtcNow;
            caseEntity.UpdatedBy = "System";

            if (request.Status == CaseStatus.Resolved)
            {
                caseEntity.ResolvedAt = DateTime.UtcNow;
                caseEntity.Resolution = request.Resolution;
            }
            else if (request.Status == CaseStatus.Closed)
            {
                caseEntity.ClosedAt = DateTime.UtcNow;
            }

            var updatedCase = await _caseRepository.UpdateAsync(caseEntity);
            _logger.LogInformation("Updated case {CaseId} status to {Status}", id, request.Status);

            return Ok(MapToDto(updatedCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating case status {CaseId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete case
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCase(Guid id)
    {
        try
        {
            var result = await _caseRepository.DeleteAsync(id);
            if (!result)
                return NotFound($"Case with ID {id} not found");

            _logger.LogInformation("Deleted case {CaseId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting case {CaseId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private static CaseDto MapToDto(Case caseEntity) => new()
    {
        Id = caseEntity.Id,
        CustomerId = caseEntity.CustomerId,
        CaseNumber = caseEntity.CaseNumber,
        Title = caseEntity.Title,
        Description = caseEntity.Description,
        Status = caseEntity.Status,
        Priority = caseEntity.Priority,
        Category = caseEntity.Category,
        SubCategory = caseEntity.SubCategory,
        CreatedAt = caseEntity.CreatedAt,
        CreatedBy = caseEntity.CreatedBy
    };
}

public record CreateCaseRequest(
    Guid CustomerId,
    string Title,
    string Description,
    CasePriority Priority,
    string Category,
    string? SubCategory
);

public record UpdateCaseStatusRequest(
    CaseStatus Status,
    string? Resolution
);
