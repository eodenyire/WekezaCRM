using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Infrastructure.Data;

namespace WekezaCRM.Infrastructure.Repositories;

public class WhatsAppRepository : IWhatsAppRepository
{
    private readonly CRMDbContext _context;

    public WhatsAppRepository(CRMDbContext context)
    {
        _context = context;
    }

    public async Task<WhatsAppMessage?> GetByIdAsync(Guid id)
    {
        return await _context.WhatsAppMessages
            .Include(m => m.Customer)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<WhatsAppMessage>> GetAllAsync()
    {
        return await _context.WhatsAppMessages
            .Include(m => m.Customer)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WhatsAppMessage>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.WhatsAppMessages
            .Include(m => m.Customer)
            .Where(m => m.CustomerId == customerId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WhatsAppMessage>> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _context.WhatsAppMessages
            .Include(m => m.Customer)
            .Where(m => m.PhoneNumber == phoneNumber)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<WhatsAppMessage> CreateAsync(WhatsAppMessage message)
    {
        _context.WhatsAppMessages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<WhatsAppMessage> UpdateAsync(WhatsAppMessage message)
    {
        _context.WhatsAppMessages.Update(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var message = await _context.WhatsAppMessages.FindAsync(id);
        if (message == null) return false;

        _context.WhatsAppMessages.Remove(message);
        await _context.SaveChangesAsync();
        return true;
    }
}
