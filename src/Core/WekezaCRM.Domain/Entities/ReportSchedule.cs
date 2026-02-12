using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class ReportSchedule : BaseEntity
{
    public Guid ReportTemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ReportScheduleFrequency Frequency { get; set; }
    public string? ScheduleExpression { get; set; }
    public DateTime? NextRunDate { get; set; }
    public DateTime? LastRunDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string Recipients { get; set; } = string.Empty;
    public ReportFormat OutputFormat { get; set; }
    public string? Parameters { get; set; }

    // Navigation properties
    public ReportTemplate ReportTemplate { get; set; } = null!;
}
