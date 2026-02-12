using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Infrastructure.Data;

namespace WekezaCRM.Infrastructure.Repositories;

public class InteractionRepository : IInteractionRepository
{
    private readonly CRMDbContext _context;

    public InteractionRepository(CRMDbContext context)
    {
        _context = context;
    }

    public async Task<Interaction?> GetByIdAsync(Guid id)
    {
        return await _context.Interactions
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Interaction>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Interactions
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.InteractionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Interaction>> GetAllAsync()
    {
        return await _context.Interactions
            .Include(i => i.Customer)
            .OrderByDescending(i => i.InteractionDate)
            .ToListAsync();
    }

    public async Task<Interaction> CreateAsync(Interaction interaction)
    {
        _context.Interactions.Add(interaction);
        await _context.SaveChangesAsync();
        return interaction;
    }

    public async Task<Interaction> UpdateAsync(Interaction interaction)
    {
        _context.Interactions.Update(interaction);
        await _context.SaveChangesAsync();
        return interaction;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var interaction = await _context.Interactions.FindAsync(id);
        if (interaction == null)
            return false;

        _context.Interactions.Remove(interaction);
        await _context.SaveChangesAsync();
        return true;
    }
}
