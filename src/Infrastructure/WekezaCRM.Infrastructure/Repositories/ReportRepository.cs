using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Infrastructure.Data;

namespace WekezaCRM.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly CRMDbContext _context;

    public ReportRepository(CRMDbContext context)
    {
        _context = context;
    }

    // Report Templates
    public async Task<ReportTemplate?> GetTemplateByIdAsync(Guid id)
    {
        return await _context.ReportTemplates
            .Include(t => t.ReportSchedules)
            .Include(t => t.GeneratedReports)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<ReportTemplate>> GetAllTemplatesAsync()
    {
        return await _context.ReportTemplates.ToListAsync();
    }

    public async Task<IEnumerable<ReportTemplate>> GetActiveTemplatesAsync()
    {
        return await _context.ReportTemplates
            .Where(t => t.IsActive)
            .ToListAsync();
    }

    public async Task<ReportTemplate> CreateTemplateAsync(ReportTemplate template)
    {
        _context.ReportTemplates.Add(template);
        await _context.SaveChangesAsync();
        return template;
    }

    public async Task<ReportTemplate> UpdateTemplateAsync(ReportTemplate template)
    {
        _context.ReportTemplates.Update(template);
        await _context.SaveChangesAsync();
        return template;
    }

    public async Task<bool> DeleteTemplateAsync(Guid id)
    {
        var template = await _context.ReportTemplates.FindAsync(id);
        if (template == null) return false;

        _context.ReportTemplates.Remove(template);
        await _context.SaveChangesAsync();
        return true;
    }

    // Report Schedules
    public async Task<ReportSchedule?> GetScheduleByIdAsync(Guid id)
    {
        return await _context.ReportSchedules
            .Include(s => s.ReportTemplate)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<ReportSchedule>> GetAllSchedulesAsync()
    {
        return await _context.ReportSchedules
            .Include(s => s.ReportTemplate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ReportSchedule>> GetSchedulesByTemplateIdAsync(Guid templateId)
    {
        return await _context.ReportSchedules
            .Where(s => s.ReportTemplateId == templateId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ReportSchedule>> GetDueSchedulesAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.ReportSchedules
            .Include(s => s.ReportTemplate)
            .Where(s => s.IsActive && s.NextRunDate.HasValue && s.NextRunDate.Value <= now)
            .ToListAsync();
    }

    public async Task<ReportSchedule> CreateScheduleAsync(ReportSchedule schedule)
    {
        _context.ReportSchedules.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<ReportSchedule> UpdateScheduleAsync(ReportSchedule schedule)
    {
        _context.ReportSchedules.Update(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<bool> DeleteScheduleAsync(Guid id)
    {
        var schedule = await _context.ReportSchedules.FindAsync(id);
        if (schedule == null) return false;

        _context.ReportSchedules.Remove(schedule);
        await _context.SaveChangesAsync();
        return true;
    }

    // Generated Reports
    public async Task<GeneratedReport?> GetGeneratedReportByIdAsync(Guid id)
    {
        return await _context.GeneratedReports
            .Include(r => r.ReportTemplate)
            .Include(r => r.ReportSchedule)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<GeneratedReport>> GetAllGeneratedReportsAsync()
    {
        return await _context.GeneratedReports
            .Include(r => r.ReportTemplate)
            .OrderByDescending(r => r.GeneratedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<GeneratedReport>> GetGeneratedReportsByTemplateIdAsync(Guid templateId)
    {
        return await _context.GeneratedReports
            .Where(r => r.ReportTemplateId == templateId)
            .OrderByDescending(r => r.GeneratedDate)
            .ToListAsync();
    }

    public async Task<GeneratedReport> CreateGeneratedReportAsync(GeneratedReport report)
    {
        _context.GeneratedReports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task<bool> DeleteGeneratedReportAsync(Guid id)
    {
        var report = await _context.GeneratedReports.FindAsync(id);
        if (report == null) return false;

        _context.GeneratedReports.Remove(report);
        await _context.SaveChangesAsync();
        return true;
    }
}
