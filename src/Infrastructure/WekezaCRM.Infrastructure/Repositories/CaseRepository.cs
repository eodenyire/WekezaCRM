using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Infrastructure.Data;

namespace WekezaCRM.Infrastructure.Repositories;

public class CaseRepository : ICaseRepository
{
    private readonly CRMDbContext _context;

    public CaseRepository(CRMDbContext context)
    {
        _context = context;
    }

    public async Task<Case?> GetByIdAsync(Guid id)
    {
        return await _context.Cases
            .Include(c => c.Customer)
            .Include(c => c.CaseNotes)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Case>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Cases
            .Where(c => c.CustomerId == customerId)
            .Include(c => c.CaseNotes)
            .ToListAsync();
    }

    public async Task<IEnumerable<Case>> GetAllAsync()
    {
        return await _context.Cases
            .Include(c => c.Customer)
            .ToListAsync();
    }

    public async Task<Case> CreateAsync(Case caseEntity)
    {
        _context.Cases.Add(caseEntity);
        await _context.SaveChangesAsync();
        return caseEntity;
    }

    public async Task<Case> UpdateAsync(Case caseEntity)
    {
        _context.Cases.Update(caseEntity);
        await _context.SaveChangesAsync();
        return caseEntity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var caseEntity = await _context.Cases.FindAsync(id);
        if (caseEntity == null)
            return false;

        _context.Cases.Remove(caseEntity);
        await _context.SaveChangesAsync();
        return true;
    }
}
