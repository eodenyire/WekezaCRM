namespace WekezaCRM.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid AccountId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public Account Account { get; set; } = null!;
}
