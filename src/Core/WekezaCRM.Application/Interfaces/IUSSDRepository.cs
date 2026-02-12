using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Application.Interfaces;

public interface IUSSDRepository
{
    Task<USSDSession?> GetByIdAsync(Guid id);
    Task<USSDSession?> GetBySessionIdAsync(string sessionId);
    Task<IEnumerable<USSDSession>> GetAllAsync();
    Task<IEnumerable<USSDSession>> GetByPhoneNumberAsync(string phoneNumber);
    Task<IEnumerable<USSDSession>> GetByCustomerIdAsync(Guid customerId);
    Task<USSDSession> CreateAsync(USSDSession session);
    Task<USSDSession> UpdateAsync(USSDSession session);
    Task<bool> DeleteAsync(Guid id);
}
