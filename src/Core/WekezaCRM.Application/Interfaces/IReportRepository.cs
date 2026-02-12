using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Application.Interfaces;

public interface IReportRepository
{
    // Report Templates
    Task<ReportTemplate?> GetTemplateByIdAsync(Guid id);
    Task<IEnumerable<ReportTemplate>> GetAllTemplatesAsync();
    Task<IEnumerable<ReportTemplate>> GetActiveTemplatesAsync();
    Task<ReportTemplate> CreateTemplateAsync(ReportTemplate template);
    Task<ReportTemplate> UpdateTemplateAsync(ReportTemplate template);
    Task<bool> DeleteTemplateAsync(Guid id);

    // Report Schedules
    Task<ReportSchedule?> GetScheduleByIdAsync(Guid id);
    Task<IEnumerable<ReportSchedule>> GetAllSchedulesAsync();
    Task<IEnumerable<ReportSchedule>> GetSchedulesByTemplateIdAsync(Guid templateId);
    Task<IEnumerable<ReportSchedule>> GetDueSchedulesAsync();
    Task<ReportSchedule> CreateScheduleAsync(ReportSchedule schedule);
    Task<ReportSchedule> UpdateScheduleAsync(ReportSchedule schedule);
    Task<bool> DeleteScheduleAsync(Guid id);

    // Generated Reports
    Task<GeneratedReport?> GetGeneratedReportByIdAsync(Guid id);
    Task<IEnumerable<GeneratedReport>> GetAllGeneratedReportsAsync();
    Task<IEnumerable<GeneratedReport>> GetGeneratedReportsByTemplateIdAsync(Guid templateId);
    Task<GeneratedReport> CreateGeneratedReportAsync(GeneratedReport report);
    Task<bool> DeleteGeneratedReportAsync(Guid id);
}
