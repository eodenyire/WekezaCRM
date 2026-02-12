using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid? UserId { get; set; }
    public Guid? CustomerId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? ActionUrl { get; set; }
    public string? Metadata { get; set; }

    // Navigation properties
    public Customer? Customer { get; set; }
}
