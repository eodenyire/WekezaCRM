using WekezaCRM.Domain.Entities;
using WekezaCRM.Application.DTOs;

namespace WekezaCRM.Application.Interfaces;

public interface IAnalyticsRepository
{
    Task<AnalyticsReport?> GetByIdAsync(Guid id);
    Task<IEnumerable<AnalyticsReport>> GetAllAsync();
    Task<AnalyticsReport> CreateAsync(AnalyticsReport report);
    
    // Analytics calculation methods
    Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<CaseAnalyticsDto> GetCaseAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<InteractionAnalyticsDto> GetInteractionAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
}
