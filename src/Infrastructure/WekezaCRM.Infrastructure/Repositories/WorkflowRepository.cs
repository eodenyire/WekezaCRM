using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Infrastructure.Data;

namespace WekezaCRM.Infrastructure.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly CRMDbContext _context;

    public WorkflowRepository(CRMDbContext context)
    {
        _context = context;
    }

    // Workflow Definition methods
    public async Task<WorkflowDefinition?> GetDefinitionByIdAsync(Guid id)
    {
        return await _context.WorkflowDefinitions
            .Include(w => w.WorkflowInstances)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync()
    {
        return await _context.WorkflowDefinitions.ToListAsync();
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetActiveDefinitionsAsync()
    {
        return await _context.WorkflowDefinitions
            .Where(w => w.IsActive)
            .OrderBy(w => w.ExecutionOrder)
            .ToListAsync();
    }

    public async Task<WorkflowDefinition> CreateDefinitionAsync(WorkflowDefinition definition)
    {
        _context.WorkflowDefinitions.Add(definition);
        await _context.SaveChangesAsync();
        return definition;
    }

    public async Task<WorkflowDefinition> UpdateDefinitionAsync(WorkflowDefinition definition)
    {
        _context.WorkflowDefinitions.Update(definition);
        await _context.SaveChangesAsync();
        return definition;
    }

    public async Task<bool> DeleteDefinitionAsync(Guid id)
    {
        var definition = await _context.WorkflowDefinitions.FindAsync(id);
        if (definition == null) return false;

        _context.WorkflowDefinitions.Remove(definition);
        await _context.SaveChangesAsync();
        return true;
    }

    // Workflow Instance methods
    public async Task<WorkflowInstance?> GetInstanceByIdAsync(Guid id)
    {
        return await _context.WorkflowInstances
            .Include(w => w.WorkflowDefinition)
            .Include(w => w.Customer)
            .Include(w => w.Case)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync()
    {
        return await _context.WorkflowInstances
            .Include(w => w.WorkflowDefinition)
            .OrderByDescending(w => w.StartedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkflowInstance>> GetInstancesByCustomerIdAsync(Guid customerId)
    {
        return await _context.WorkflowInstances
            .Include(w => w.WorkflowDefinition)
            .Where(w => w.CustomerId == customerId)
            .OrderByDescending(w => w.StartedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkflowInstance>> GetInstancesByCaseIdAsync(Guid caseId)
    {
        return await _context.WorkflowInstances
            .Include(w => w.WorkflowDefinition)
            .Where(w => w.CaseId == caseId)
            .OrderByDescending(w => w.StartedAt)
            .ToListAsync();
    }

    public async Task<WorkflowInstance> CreateInstanceAsync(WorkflowInstance instance)
    {
        _context.WorkflowInstances.Add(instance);
        await _context.SaveChangesAsync();
        return instance;
    }

    public async Task<WorkflowInstance> UpdateInstanceAsync(WorkflowInstance instance)
    {
        _context.WorkflowInstances.Update(instance);
        await _context.SaveChangesAsync();
        return instance;
    }
}
