using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkflowsController : ControllerBase
{
    private readonly IWorkflowRepository _repository;
    private readonly ILogger<WorkflowsController> _logger;

    public WorkflowsController(
        IWorkflowRepository repository,
        ILogger<WorkflowsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // Workflow Definition endpoints
    
    /// <summary>
    /// Get all workflow definitions
    /// </summary>
    [HttpGet("definitions")]
    public async Task<ActionResult<IEnumerable<WorkflowDefinitionDto>>> GetAllDefinitions()
    {
        try
        {
            var definitions = await _repository.GetAllDefinitionsAsync();
            var definitionDtos = definitions.Select(MapDefinitionToDto);
            return Ok(definitionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow definitions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get active workflow definitions
    /// </summary>
    [HttpGet("definitions/active")]
    public async Task<ActionResult<IEnumerable<WorkflowDefinitionDto>>> GetActiveDefinitions()
    {
        try
        {
            var definitions = await _repository.GetActiveDefinitionsAsync();
            var definitionDtos = definitions.Select(MapDefinitionToDto);
            return Ok(definitionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active workflow definitions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get workflow definition by ID
    /// </summary>
    [HttpGet("definitions/{id}")]
    public async Task<ActionResult<WorkflowDefinitionDto>> GetDefinition(Guid id)
    {
        try
        {
            var definition = await _repository.GetDefinitionByIdAsync(id);
            if (definition == null)
                return NotFound($"Workflow definition with ID {id} not found");

            return Ok(MapDefinitionToDto(definition));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow definition {DefinitionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a workflow definition
    /// </summary>
    [HttpPost("definitions")]
    public async Task<ActionResult<WorkflowDefinitionDto>> CreateDefinition([FromBody] CreateWorkflowDefinitionRequest request)
    {
        try
        {
            var definition = new WorkflowDefinition
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                TriggerType = request.TriggerType,
                TriggerConditions = request.TriggerConditions,
                Actions = request.Actions,
                IsActive = request.IsActive,
                ExecutionOrder = request.ExecutionOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var created = await _repository.CreateDefinitionAsync(definition);
            _logger.LogInformation("Created workflow definition {DefinitionId}", created.Id);

            return CreatedAtAction(
                nameof(GetDefinition),
                new { id = created.Id },
                MapDefinitionToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workflow definition");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update a workflow definition
    /// </summary>
    [HttpPut("definitions/{id}")]
    public async Task<ActionResult<WorkflowDefinitionDto>> UpdateDefinition(Guid id, [FromBody] UpdateWorkflowDefinitionRequest request)
    {
        try
        {
            var definition = await _repository.GetDefinitionByIdAsync(id);
            if (definition == null)
                return NotFound($"Workflow definition with ID {id} not found");

            if (request.Name != null) definition.Name = request.Name;
            if (request.Description != null) definition.Description = request.Description;
            if (request.IsActive.HasValue) definition.IsActive = request.IsActive.Value;
            definition.UpdatedAt = DateTime.UtcNow;
            definition.UpdatedBy = "System";

            var updated = await _repository.UpdateDefinitionAsync(definition);
            _logger.LogInformation("Updated workflow definition {DefinitionId}", id);

            return Ok(MapDefinitionToDto(updated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workflow definition {DefinitionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a workflow definition
    /// </summary>
    [HttpDelete("definitions/{id}")]
    public async Task<ActionResult> DeleteDefinition(Guid id)
    {
        try
        {
            var result = await _repository.DeleteDefinitionAsync(id);
            if (!result)
                return NotFound($"Workflow definition with ID {id} not found");

            _logger.LogInformation("Deleted workflow definition {DefinitionId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting workflow definition {DefinitionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    // Workflow Instance endpoints

    /// <summary>
    /// Get all workflow instances
    /// </summary>
    [HttpGet("instances")]
    public async Task<ActionResult<IEnumerable<WorkflowInstanceDto>>> GetAllInstances()
    {
        try
        {
            var instances = await _repository.GetAllInstancesAsync();
            var instanceDtos = instances.Select(MapInstanceToDto);
            return Ok(instanceDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow instances");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get workflow instance by ID
    /// </summary>
    [HttpGet("instances/{id}")]
    public async Task<ActionResult<WorkflowInstanceDto>> GetInstance(Guid id)
    {
        try
        {
            var instance = await _repository.GetInstanceByIdAsync(id);
            if (instance == null)
                return NotFound($"Workflow instance with ID {id} not found");

            return Ok(MapInstanceToDto(instance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow instance {InstanceId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get workflow instances for a customer
    /// </summary>
    [HttpGet("instances/customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<WorkflowInstanceDto>>> GetInstancesByCustomer(Guid customerId)
    {
        try
        {
            var instances = await _repository.GetInstancesByCustomerIdAsync(customerId);
            var instanceDtos = instances.Select(MapInstanceToDto);
            return Ok(instanceDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow instances for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Trigger a workflow
    /// </summary>
    [HttpPost("instances/trigger")]
    public async Task<ActionResult<WorkflowInstanceDto>> TriggerWorkflow([FromBody] TriggerWorkflowRequest request)
    {
        try
        {
            var instance = new WorkflowInstance
            {
                Id = Guid.NewGuid(),
                WorkflowDefinitionId = request.WorkflowDefinitionId,
                CustomerId = request.CustomerId,
                CaseId = request.CaseId,
                Status = WorkflowStatus.Active,
                StartedAt = DateTime.UtcNow,
                CurrentStep = "Started",
                ExecutionContext = request.Context,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var created = await _repository.CreateInstanceAsync(instance);
            _logger.LogInformation("Triggered workflow instance {InstanceId}", created.Id);

            return CreatedAtAction(
                nameof(GetInstance),
                new { id = created.Id },
                MapInstanceToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering workflow");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update workflow instance status
    /// </summary>
    [HttpPut("instances/{id}/status")]
    public async Task<ActionResult<WorkflowInstanceDto>> UpdateInstanceStatus(Guid id, [FromBody] UpdateWorkflowStatusRequest request)
    {
        try
        {
            var instance = await _repository.GetInstanceByIdAsync(id);
            if (instance == null)
                return NotFound($"Workflow instance with ID {id} not found");

            instance.Status = request.Status;
            instance.CurrentStep = request.CurrentStep;
            instance.ErrorMessage = request.ErrorMessage;
            
            if (request.Status == WorkflowStatus.Completed || request.Status == WorkflowStatus.Failed || request.Status == WorkflowStatus.Cancelled)
            {
                instance.CompletedAt = DateTime.UtcNow;
            }

            instance.UpdatedAt = DateTime.UtcNow;
            instance.UpdatedBy = "System";

            var updated = await _repository.UpdateInstanceAsync(instance);
            _logger.LogInformation("Updated workflow instance {InstanceId} status to {Status}", id, request.Status);

            return Ok(MapInstanceToDto(updated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workflow instance {InstanceId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private static WorkflowDefinitionDto MapDefinitionToDto(WorkflowDefinition definition) => new()
    {
        Id = definition.Id,
        Name = definition.Name,
        Description = definition.Description,
        TriggerType = definition.TriggerType,
        IsActive = definition.IsActive,
        ExecutionOrder = definition.ExecutionOrder
    };

    private static WorkflowInstanceDto MapInstanceToDto(WorkflowInstance instance) => new()
    {
        Id = instance.Id,
        WorkflowDefinitionId = instance.WorkflowDefinitionId,
        CustomerId = instance.CustomerId,
        CaseId = instance.CaseId,
        Status = instance.Status,
        StartedAt = instance.StartedAt,
        CompletedAt = instance.CompletedAt,
        CurrentStep = instance.CurrentStep,
        ErrorMessage = instance.ErrorMessage
    };
}

public record CreateWorkflowDefinitionRequest(
    string Name,
    string Description,
    string TriggerType,
    string TriggerConditions,
    string Actions,
    bool IsActive,
    int ExecutionOrder
);

public record UpdateWorkflowDefinitionRequest(
    string? Name,
    string? Description,
    bool? IsActive
);

public record TriggerWorkflowRequest(
    Guid WorkflowDefinitionId,
    Guid? CustomerId,
    Guid? CaseId,
    string? Context
);

public record UpdateWorkflowStatusRequest(
    WorkflowStatus Status,
    string? CurrentStep,
    string? ErrorMessage
);
