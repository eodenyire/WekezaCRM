using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Application.Interfaces;

public interface ICaseRepository
{
    Task<Case?> GetByIdAsync(Guid id);
    Task<IEnumerable<Case>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Case>> GetAllAsync();
    Task<Case> CreateAsync(Case caseEntity);
    Task<Case> UpdateAsync(Case caseEntity);
    Task<bool> DeleteAsync(Guid id);
}
