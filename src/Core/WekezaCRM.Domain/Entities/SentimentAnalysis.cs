using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class SentimentAnalysis : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid? InteractionId { get; set; }
    public Guid? CaseId { get; set; }
    public SentimentType SentimentType { get; set; }
    public decimal SentimentScore { get; set; }
    public string? TextAnalyzed { get; set; }
    public DateTime AnalyzedDate { get; set; }
    public string? KeyPhrases { get; set; }
    public string? AnalysisMetadata { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public Interaction? Interaction { get; set; }
    public Case? Case { get; set; }
}
