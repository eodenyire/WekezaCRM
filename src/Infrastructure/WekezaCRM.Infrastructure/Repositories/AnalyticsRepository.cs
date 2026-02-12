using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Infrastructure.Data;

namespace WekezaCRM.Infrastructure.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly CRMDbContext _context;

    public AnalyticsRepository(CRMDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsReport?> GetByIdAsync(Guid id)
    {
        return await _context.AnalyticsReports.FindAsync(id);
    }

    public async Task<IEnumerable<AnalyticsReport>> GetAllAsync()
    {
        return await _context.AnalyticsReports
            .OrderByDescending(r => r.GeneratedDate)
            .ToListAsync();
    }

    public async Task<AnalyticsReport> CreateAsync(AnalyticsReport report)
    {
        _context.AnalyticsReports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Customers.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(c => c.CreatedAt >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(c => c.CreatedAt <= endDate.Value);

        var customers = await query.ToListAsync();

        var totalCustomers = customers.Count;
        var activeCustomers = customers.Count(c => c.IsActive);
        
        var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var newCustomersThisMonth = customers.Count(c => c.CreatedAt >= firstDayOfMonth);

        var customersBySegment = customers
            .GroupBy(c => c.Segment.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var customersByKYCStatus = customers
            .GroupBy(c => c.KYCStatus.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return new CustomerAnalyticsDto
        {
            TotalCustomers = totalCustomers,
            ActiveCustomers = activeCustomers,
            NewCustomersThisMonth = newCustomersThisMonth,
            CustomersBySegment = customersBySegment,
            CustomersByKYCStatus = customersByKYCStatus
        };
    }

    public async Task<CaseAnalyticsDto> GetCaseAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Cases.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(c => c.CreatedAt >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(c => c.CreatedAt <= endDate.Value);

        var cases = await query.ToListAsync();

        var totalCases = cases.Count;
        var openCases = cases.Count(c => c.Status == Domain.Enums.CaseStatus.Open || c.Status == Domain.Enums.CaseStatus.InProgress);
        var resolvedCases = cases.Count(c => c.Status == Domain.Enums.CaseStatus.Resolved || c.Status == Domain.Enums.CaseStatus.Closed);

        var resolvedCasesWithTimes = cases
            .Where(c => c.ResolvedAt.HasValue)
            .Select(c => (c.ResolvedAt!.Value - c.CreatedAt).TotalHours)
            .ToList();

        var avgResolutionTime = resolvedCasesWithTimes.Any() 
            ? resolvedCasesWithTimes.Average() 
            : 0;

        var casesByPriority = cases
            .GroupBy(c => c.Priority.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var casesByStatus = cases
            .GroupBy(c => c.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return new CaseAnalyticsDto
        {
            TotalCases = totalCases,
            OpenCases = openCases,
            ResolvedCases = resolvedCases,
            AverageResolutionTimeHours = avgResolutionTime,
            CasesByPriority = casesByPriority,
            CasesByStatus = casesByStatus
        };
    }

    public async Task<InteractionAnalyticsDto> GetInteractionAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Interactions.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(i => i.InteractionDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(i => i.InteractionDate <= endDate.Value);

        var interactions = await query.ToListAsync();

        var totalInteractions = interactions.Count;
        
        var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var interactionsThisMonth = interactions.Count(i => i.InteractionDate >= firstDayOfMonth);

        var interactionsByChannel = interactions
            .GroupBy(i => i.Channel.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var interactionsWithDuration = interactions
            .Where(i => i.DurationMinutes.HasValue)
            .Select(i => i.DurationMinutes!.Value)
            .ToList();

        var avgDuration = interactionsWithDuration.Any() 
            ? interactionsWithDuration.Average() 
            : 0;

        return new InteractionAnalyticsDto
        {
            TotalInteractions = totalInteractions,
            InteractionsThisMonth = interactionsThisMonth,
            InteractionsByChannel = interactionsByChannel,
            AverageInteractionDurationMinutes = avgDuration
        };
    }
}
