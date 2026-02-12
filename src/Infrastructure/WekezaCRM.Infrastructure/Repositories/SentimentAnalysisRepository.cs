using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Infrastructure.Data;

namespace WekezaCRM.Infrastructure.Repositories;

public class SentimentAnalysisRepository : ISentimentAnalysisRepository
{
    private readonly CRMDbContext _context;

    public SentimentAnalysisRepository(CRMDbContext context)
    {
        _context = context;
    }

    public async Task<SentimentAnalysis?> GetByIdAsync(Guid id)
    {
        return await _context.SentimentAnalyses
            .Include(s => s.Customer)
            .Include(s => s.Interaction)
            .Include(s => s.Case)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<SentimentAnalysis>> GetAllAsync()
    {
        return await _context.SentimentAnalyses
            .Include(s => s.Customer)
            .OrderByDescending(s => s.AnalyzedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<SentimentAnalysis>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.SentimentAnalyses
            .Include(s => s.Customer)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.AnalyzedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<SentimentAnalysis>> GetByInteractionIdAsync(Guid interactionId)
    {
        return await _context.SentimentAnalyses
            .Include(s => s.Customer)
            .Include(s => s.Interaction)
            .Where(s => s.InteractionId == interactionId)
            .ToListAsync();
    }

    public async Task<IEnumerable<SentimentAnalysis>> GetByCaseIdAsync(Guid caseId)
    {
        return await _context.SentimentAnalyses
            .Include(s => s.Customer)
            .Include(s => s.Case)
            .Where(s => s.CaseId == caseId)
            .ToListAsync();
    }

    public async Task<SentimentAnalysis> CreateAsync(SentimentAnalysis sentiment)
    {
        _context.SentimentAnalyses.Add(sentiment);
        await _context.SaveChangesAsync();
        return sentiment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sentiment = await _context.SentimentAnalyses.FindAsync(id);
        if (sentiment == null) return false;

        _context.SentimentAnalyses.Remove(sentiment);
        await _context.SaveChangesAsync();
        return true;
    }
}
