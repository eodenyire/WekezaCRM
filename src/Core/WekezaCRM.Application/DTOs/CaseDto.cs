using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Application.DTOs;

public record CaseDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string CaseNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public CaseStatus Status { get; init; }
    public CasePriority Priority { get; init; }
    public string Category { get; init; } = string.Empty;
    public string SubCategory { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
}
