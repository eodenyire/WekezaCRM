using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class ReportTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string QueryDefinition { get; set; } = string.Empty;
    public string ParametersSchema { get; set; } = string.Empty;
    public ReportFormat DefaultFormat { get; set; }
    public bool IsActive { get; set; } = true;
    public string? LayoutTemplate { get; set; }

    // Navigation properties
    public ICollection<ReportSchedule> ReportSchedules { get; set; } = new List<ReportSchedule>();
    public ICollection<GeneratedReport> GeneratedReports { get; set; } = new List<GeneratedReport>();
}
