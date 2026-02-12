using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class NextBestAction : BaseEntity
{
    public Guid CustomerId { get; set; }
    public ActionType ActionType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; }
    public string? RecommendedProduct { get; set; }
    public DateTime RecommendedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool IsCompleted { get; set; }
    public string? Outcome { get; set; }
    public string? AIModelVersion { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
}
