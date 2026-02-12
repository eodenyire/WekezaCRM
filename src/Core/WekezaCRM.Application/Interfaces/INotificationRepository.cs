using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Application.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id);
    Task<IEnumerable<Notification>> GetAllAsync();
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId);
    Task<Notification> CreateAsync(Notification notification);
    Task<Notification> UpdateAsync(Notification notification);
    Task<bool> MarkAsReadAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}
