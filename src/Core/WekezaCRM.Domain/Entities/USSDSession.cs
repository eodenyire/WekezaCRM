using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class USSDSession : BaseEntity
{
    public string SessionId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public USSDSessionStatus Status { get; set; }
    public string CurrentMenu { get; set; } = string.Empty;
    public string MenuHistory { get; set; } = string.Empty;
    public string? UserInput { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? TransactionData { get; set; }
    public string? ErrorMessage { get; set; }

    // Navigation properties
    public Customer? Customer { get; set; }
}
