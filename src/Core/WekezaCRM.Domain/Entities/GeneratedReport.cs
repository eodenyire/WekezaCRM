using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class GeneratedReport : BaseEntity
{
    public Guid ReportTemplateId { get; set; }
    public Guid? ReportScheduleId { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    public ReportFormat Format { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string? Parameters { get; set; }
    public Guid? GeneratedByUserId { get; set; }
    public int RecordCount { get; set; }
    public bool IsDownloaded { get; set; }
    public DateTime? DownloadedAt { get; set; }

    // Navigation properties
    public ReportTemplate ReportTemplate { get; set; } = null!;
    public ReportSchedule? ReportSchedule { get; set; }
}
