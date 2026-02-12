using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Application.Interfaces;

public interface ISentimentAnalysisRepository
{
    Task<SentimentAnalysis?> GetByIdAsync(Guid id);
    Task<IEnumerable<SentimentAnalysis>> GetAllAsync();
    Task<IEnumerable<SentimentAnalysis>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<SentimentAnalysis>> GetByInteractionIdAsync(Guid interactionId);
    Task<IEnumerable<SentimentAnalysis>> GetByCaseIdAsync(Guid caseId);
    Task<SentimentAnalysis> CreateAsync(SentimentAnalysis sentiment);
    Task<bool> DeleteAsync(Guid id);
}
