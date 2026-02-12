using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Application.DTOs;

public class WorkflowDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TriggerType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ExecutionOrder { get; set; }
}

public class WorkflowInstanceDto
{
    public Guid Id { get; set; }
    public Guid WorkflowDefinitionId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? CaseId { get; set; }
    public WorkflowStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CurrentStep { get; set; }
    public string? ErrorMessage { get; set; }
}
