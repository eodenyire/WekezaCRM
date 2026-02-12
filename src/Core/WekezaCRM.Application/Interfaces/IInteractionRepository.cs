using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Application.Interfaces;

public interface IInteractionRepository
{
    Task<Interaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Interaction>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Interaction>> GetAllAsync();
    Task<Interaction> CreateAsync(Interaction interaction);
    Task<Interaction> UpdateAsync(Interaction interaction);
    Task<bool> DeleteAsync(Guid id);
}
