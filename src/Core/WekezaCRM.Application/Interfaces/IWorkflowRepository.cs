using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Application.Interfaces;

public interface IWorkflowRepository
{
    // Workflow Definitions
    Task<WorkflowDefinition?> GetDefinitionByIdAsync(Guid id);
    Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync();
    Task<IEnumerable<WorkflowDefinition>> GetActiveDefinitionsAsync();
    Task<WorkflowDefinition> CreateDefinitionAsync(WorkflowDefinition definition);
    Task<WorkflowDefinition> UpdateDefinitionAsync(WorkflowDefinition definition);
    Task<bool> DeleteDefinitionAsync(Guid id);

    // Workflow Instances
    Task<WorkflowInstance?> GetInstanceByIdAsync(Guid id);
    Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync();
    Task<IEnumerable<WorkflowInstance>> GetInstancesByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<WorkflowInstance>> GetInstancesByCaseIdAsync(Guid caseId);
    Task<WorkflowInstance> CreateInstanceAsync(WorkflowInstance instance);
    Task<WorkflowInstance> UpdateInstanceAsync(WorkflowInstance instance);
}
