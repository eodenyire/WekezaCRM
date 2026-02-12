using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Infrastructure.Data;

namespace WekezaCRM.Infrastructure.Repositories;

public class USSDRepository : IUSSDRepository
{
    private readonly CRMDbContext _context;

    public USSDRepository(CRMDbContext context)
    {
        _context = context;
    }

    public async Task<USSDSession?> GetByIdAsync(Guid id)
    {
        return await _context.USSDSessions
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<USSDSession?> GetBySessionIdAsync(string sessionId)
    {
        return await _context.USSDSessions
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
    }

    public async Task<IEnumerable<USSDSession>> GetAllAsync()
    {
        return await _context.USSDSessions
            .Include(s => s.Customer)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<USSDSession>> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _context.USSDSessions
            .Include(s => s.Customer)
            .Where(s => s.PhoneNumber == phoneNumber)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<USSDSession>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.USSDSessions
            .Include(s => s.Customer)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync();
    }

    public async Task<USSDSession> CreateAsync(USSDSession session)
    {
        _context.USSDSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<USSDSession> UpdateAsync(USSDSession session)
    {
        _context.USSDSessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var session = await _context.USSDSessions.FindAsync(id);
        if (session == null) return false;

        _context.USSDSessions.Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }
}
