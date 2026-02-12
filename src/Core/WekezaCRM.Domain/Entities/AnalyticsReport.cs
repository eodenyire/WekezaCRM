namespace WekezaCRM.Domain.Entities;

public class AnalyticsReport : BaseEntity
{
    public string ReportName { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ReportData { get; set; } = string.Empty;
    public string? Parameters { get; set; }
    public Guid? GeneratedByUserId { get; set; }
}
