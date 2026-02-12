using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Infrastructure.Data;

namespace WekezaCRM.Infrastructure.Repositories;

public class NextBestActionRepository : INextBestActionRepository
{
    private readonly CRMDbContext _context;

    public NextBestActionRepository(CRMDbContext context)
    {
        _context = context;
    }

    public async Task<NextBestAction?> GetByIdAsync(Guid id)
    {
        return await _context.NextBestActions
            .Include(n => n.Customer)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<NextBestAction>> GetAllAsync()
    {
        return await _context.NextBestActions
            .Include(n => n.Customer)
            .ToListAsync();
    }

    public async Task<IEnumerable<NextBestAction>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.NextBestActions
            .Include(n => n.Customer)
            .Where(n => n.CustomerId == customerId)
            .OrderByDescending(n => n.RecommendedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<NextBestAction>> GetPendingByCustomerIdAsync(Guid customerId)
    {
        return await _context.NextBestActions
            .Include(n => n.Customer)
            .Where(n => n.CustomerId == customerId && !n.IsCompleted)
            .OrderByDescending(n => n.ConfidenceScore)
            .ToListAsync();
    }

    public async Task<NextBestAction> CreateAsync(NextBestAction action)
    {
        _context.NextBestActions.Add(action);
        await _context.SaveChangesAsync();
        return action;
    }

    public async Task<NextBestAction> UpdateAsync(NextBestAction action)
    {
        _context.NextBestActions.Update(action);
        await _context.SaveChangesAsync();
        return action;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var action = await _context.NextBestActions.FindAsync(id);
        if (action == null) return false;

        _context.NextBestActions.Remove(action);
        await _context.SaveChangesAsync();
        return true;
    }
}
