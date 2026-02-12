using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Application.Interfaces;

public interface INextBestActionRepository
{
    Task<NextBestAction?> GetByIdAsync(Guid id);
    Task<IEnumerable<NextBestAction>> GetAllAsync();
    Task<IEnumerable<NextBestAction>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<NextBestAction>> GetPendingByCustomerIdAsync(Guid customerId);
    Task<NextBestAction> CreateAsync(NextBestAction action);
    Task<NextBestAction> UpdateAsync(NextBestAction action);
    Task<bool> DeleteAsync(Guid id);
}
