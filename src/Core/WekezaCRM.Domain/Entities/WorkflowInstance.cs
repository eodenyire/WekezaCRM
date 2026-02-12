using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class WorkflowInstance : BaseEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? CaseId { get; set; }
    public WorkflowStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CurrentStep { get; set; }
    public string? ExecutionContext { get; set; }
    public string? ErrorMessage { get; set; }

    // Navigation properties
    public WorkflowDefinition WorkflowDefinition { get; set; } = null!;
    public Customer? Customer { get; set; }
    public Case? Case { get; set; }
}
