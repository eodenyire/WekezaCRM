using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NextBestActionsController : ControllerBase
{
    private readonly INextBestActionRepository _repository;
    private readonly ILogger<NextBestActionsController> _logger;

    public NextBestActionsController(
        INextBestActionRepository repository,
        ILogger<NextBestActionsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Get all next best actions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NextBestActionDto>>> GetAllActions()
    {
        try
        {
            var actions = await _repository.GetAllAsync();
            var actionDtos = actions.Select(MapToDto);
            return Ok(actionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving next best actions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get next best action by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<NextBestActionDto>> GetAction(Guid id)
    {
        try
        {
            var action = await _repository.GetByIdAsync(id);
            if (action == null)
                return NotFound($"Next best action with ID {id} not found");

            return Ok(MapToDto(action));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving next best action {ActionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get next best actions for a customer
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<NextBestActionDto>>> GetByCustomer(Guid customerId)
    {
        try
        {
            var actions = await _repository.GetByCustomerIdAsync(customerId);
            var actionDtos = actions.Select(MapToDto);
            return Ok(actionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving actions for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get pending next best actions for a customer
    /// </summary>
    [HttpGet("customer/{customerId}/pending")]
    public async Task<ActionResult<IEnumerable<NextBestActionDto>>> GetPendingByCustomer(Guid customerId)
    {
        try
        {
            var actions = await _repository.GetPendingByCustomerIdAsync(customerId);
            var actionDtos = actions.Select(MapToDto);
            return Ok(actionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending actions for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Generate next best actions for a customer (AI simulation)
    /// </summary>
    [HttpPost("generate/{customerId}")]
    public async Task<ActionResult<IEnumerable<NextBestActionDto>>> GenerateActions(Guid customerId)
    {
        try
        {
            // Simulated AI recommendations - in production this would call ML service
            var recommendations = new[]
            {
                new NextBestAction
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ActionType = ActionType.ProductRecommendation,
                    Title = "Recommend Premium Savings Account",
                    Description = "Customer profile indicates readiness for premium account upgrade",
                    ConfidenceScore = 0.85m,
                    RecommendedProduct = "Premium Savings Account",
                    RecommendedDate = DateTime.UtcNow,
                    IsCompleted = false,
                    AIModelVersion = "v1.0-simulation",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "AI-Engine"
                },
                new NextBestAction
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ActionType = ActionType.FollowUpCall,
                    Title = "Schedule Follow-up Call",
                    Description = "Customer engagement score suggests scheduling a relationship building call",
                    ConfidenceScore = 0.72m,
                    RecommendedDate = DateTime.UtcNow,
                    IsCompleted = false,
                    AIModelVersion = "v1.0-simulation",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "AI-Engine"
                }
            };

            var createdActions = new List<NextBestActionDto>();
            foreach (var action in recommendations)
            {
                var created = await _repository.CreateAsync(action);
                createdActions.Add(MapToDto(created));
            }

            _logger.LogInformation("Generated {Count} next best actions for customer {CustomerId}", 
                createdActions.Count, customerId);

            return Ok(createdActions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating actions for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Complete a next best action
    /// </summary>
    [HttpPut("{id}/complete")]
    public async Task<ActionResult<NextBestActionDto>> CompleteAction(Guid id, [FromBody] CompleteActionRequest request)
    {
        try
        {
            var action = await _repository.GetByIdAsync(id);
            if (action == null)
                return NotFound($"Next best action with ID {id} not found");

            action.IsCompleted = true;
            action.CompletedDate = DateTime.UtcNow;
            action.Outcome = request.Outcome;
            action.UpdatedAt = DateTime.UtcNow;
            action.UpdatedBy = "System";

            var updated = await _repository.UpdateAsync(action);
            _logger.LogInformation("Completed next best action {ActionId}", id);

            return Ok(MapToDto(updated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing action {ActionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a next best action
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAction(Guid id)
    {
        try
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
                return NotFound($"Next best action with ID {id} not found");

            _logger.LogInformation("Deleted next best action {ActionId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting action {ActionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private static NextBestActionDto MapToDto(NextBestAction action) => new()
    {
        Id = action.Id,
        CustomerId = action.CustomerId,
        ActionType = action.ActionType,
        Title = action.Title,
        Description = action.Description,
        ConfidenceScore = action.ConfidenceScore,
        RecommendedProduct = action.RecommendedProduct,
        RecommendedDate = action.RecommendedDate,
        CompletedDate = action.CompletedDate,
        IsCompleted = action.IsCompleted,
        Outcome = action.Outcome
    };
}

public record CompleteActionRequest(string? Outcome);
