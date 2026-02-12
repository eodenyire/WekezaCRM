using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class WorkflowDefinition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TriggerType { get; set; } = string.Empty;
    public string TriggerConditions { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int ExecutionOrder { get; set; }

    // Navigation properties
    public ICollection<WorkflowInstance> WorkflowInstances { get; set; } = new List<WorkflowInstance>();
}
