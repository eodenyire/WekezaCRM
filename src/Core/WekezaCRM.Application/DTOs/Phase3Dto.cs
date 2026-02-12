using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Application.DTOs;

public class WhatsAppMessageDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public WhatsAppMessageType MessageType { get; set; }
    public WhatsAppMessageStatus Status { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsInbound { get; set; }
}

public class USSDSessionDto
{
    public Guid Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public USSDSessionStatus Status { get; set; }
    public string CurrentMenu { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class ReportTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public ReportFormat DefaultFormat { get; set; }
    public bool IsActive { get; set; }
}

public class ReportScheduleDto
{
    public Guid Id { get; set; }
    public Guid ReportTemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ReportScheduleFrequency Frequency { get; set; }
    public DateTime? NextRunDate { get; set; }
    public DateTime? LastRunDate { get; set; }
    public bool IsActive { get; set; }
    public ReportFormat OutputFormat { get; set; }
}

public class GeneratedReportDto
{
    public Guid Id { get; set; }
    public Guid ReportTemplateId { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    public ReportFormat Format { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public int RecordCount { get; set; }
    public bool IsDownloaded { get; set; }
}
