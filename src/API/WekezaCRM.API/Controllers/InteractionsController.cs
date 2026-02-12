using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InteractionsController : ControllerBase
{
    private readonly IInteractionRepository _interactionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<InteractionsController> _logger;

    public InteractionsController(
        IInteractionRepository interactionRepository,
        ICustomerRepository customerRepository,
        ILogger<InteractionsController> logger)
    {
        _interactionRepository = interactionRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all interactions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InteractionDto>>> GetAllInteractions()
    {
        try
        {
            var interactions = await _interactionRepository.GetAllAsync();
            var interactionDtos = interactions.Select(MapToDto);
            return Ok(interactionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving interactions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get interaction by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<InteractionDto>> GetInteraction(Guid id)
    {
        try
        {
            var interaction = await _interactionRepository.GetByIdAsync(id);
            if (interaction == null)
                return NotFound($"Interaction with ID {id} not found");

            return Ok(MapToDto(interaction));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving interaction {InteractionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get interactions by customer ID
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<InteractionDto>>> GetInteractionsByCustomer(Guid customerId)
    {
        try
        {
            var interactions = await _interactionRepository.GetByCustomerIdAsync(customerId);
            var interactionDtos = interactions.Select(MapToDto);
            return Ok(interactionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving interactions for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new interaction
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<InteractionDto>> CreateInteraction([FromBody] CreateInteractionRequest request)
    {
        try
        {
            // Verify customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
                return NotFound($"Customer with ID {request.CustomerId} not found");

            var interaction = new Interaction
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                Channel = request.Channel,
                Subject = request.Subject,
                Description = request.Description,
                InteractionDate = request.InteractionDate ?? DateTime.UtcNow,
                DurationMinutes = request.DurationMinutes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var createdInteraction = await _interactionRepository.CreateAsync(interaction);
            _logger.LogInformation("Created interaction {InteractionId} for customer {CustomerId}", 
                createdInteraction.Id, request.CustomerId);

            return CreatedAtAction(
                nameof(GetInteraction), 
                new { id = createdInteraction.Id }, 
                MapToDto(createdInteraction));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating interaction");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete interaction
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInteraction(Guid id)
    {
        try
        {
            var result = await _interactionRepository.DeleteAsync(id);
            if (!result)
                return NotFound($"Interaction with ID {id} not found");

            _logger.LogInformation("Deleted interaction {InteractionId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting interaction {InteractionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private static InteractionDto MapToDto(Interaction interaction) => new()
    {
        Id = interaction.Id,
        CustomerId = interaction.CustomerId,
        Channel = interaction.Channel,
        Subject = interaction.Subject,
        Description = interaction.Description,
        InteractionDate = interaction.InteractionDate,
        DurationMinutes = interaction.DurationMinutes
    };
}

public record CreateInteractionRequest(
    Guid CustomerId,
    InteractionChannel Channel,
    string Subject,
    string Description,
    DateTime? InteractionDate,
    int? DurationMinutes
);
