using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Application.Interfaces;

public interface IWhatsAppRepository
{
    Task<WhatsAppMessage?> GetByIdAsync(Guid id);
    Task<IEnumerable<WhatsAppMessage>> GetAllAsync();
    Task<IEnumerable<WhatsAppMessage>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<WhatsAppMessage>> GetByPhoneNumberAsync(string phoneNumber);
    Task<WhatsAppMessage> CreateAsync(WhatsAppMessage message);
    Task<WhatsAppMessage> UpdateAsync(WhatsAppMessage message);
    Task<bool> DeleteAsync(Guid id);
}
