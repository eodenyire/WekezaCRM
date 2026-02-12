using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class WhatsAppMessage : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public WhatsAppMessageType MessageType { get; set; }
    public WhatsAppMessageStatus Status { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public string? TemplateId { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? WhatsAppMessageId { get; set; }
    public bool IsInbound { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
}
