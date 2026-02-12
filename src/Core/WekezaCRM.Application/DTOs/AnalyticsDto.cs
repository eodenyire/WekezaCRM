namespace WekezaCRM.Application.DTOs;

public class AnalyticsReportDto
{
    public Guid Id { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ReportData { get; set; } = string.Empty;
}

public class CustomerAnalyticsDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public Dictionary<string, int> CustomersBySegment { get; set; } = new();
    public Dictionary<string, int> CustomersByKYCStatus { get; set; } = new();
}

public class CaseAnalyticsDto
{
    public int TotalCases { get; set; }
    public int OpenCases { get; set; }
    public int ResolvedCases { get; set; }
    public double AverageResolutionTimeHours { get; set; }
    public Dictionary<string, int> CasesByPriority { get; set; } = new();
    public Dictionary<string, int> CasesByStatus { get; set; } = new();
}

public class InteractionAnalyticsDto
{
    public int TotalInteractions { get; set; }
    public int InteractionsThisMonth { get; set; }
    public Dictionary<string, int> InteractionsByChannel { get; set; } = new();
    public double AverageInteractionDurationMinutes { get; set; }
}
