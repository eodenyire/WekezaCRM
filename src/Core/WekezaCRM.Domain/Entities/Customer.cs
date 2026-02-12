using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public CustomerSegment Segment { get; set; }
    public KYCStatus KYCStatus { get; set; }
    public string? CustomerReference { get; set; }
    public decimal? CreditScore { get; set; }
    public decimal? LifetimeValue { get; set; }
    public int? RiskScore { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
    public ICollection<Case> Cases { get; set; } = new List<Case>();
    public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
}
