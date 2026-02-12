using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CustomerId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? ActionUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
