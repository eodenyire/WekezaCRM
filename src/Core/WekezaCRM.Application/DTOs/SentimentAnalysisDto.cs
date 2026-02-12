using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Application.DTOs;

public class SentimentAnalysisDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? InteractionId { get; set; }
    public Guid? CaseId { get; set; }
    public SentimentType SentimentType { get; set; }
    public decimal SentimentScore { get; set; }
    public string? TextAnalyzed { get; set; }
    public DateTime AnalyzedDate { get; set; }
    public string? KeyPhrases { get; set; }
}
