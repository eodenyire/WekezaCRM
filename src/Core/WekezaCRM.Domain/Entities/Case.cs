using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class Case : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CaseStatus Status { get; set; }
    public CasePriority Priority { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public Guid? AssignedToUserId { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? Resolution { get; set; }
    public int? SLADurationHours { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<CaseNote> CaseNotes { get; set; } = new List<CaseNote>();
}
