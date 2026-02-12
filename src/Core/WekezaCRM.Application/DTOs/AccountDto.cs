namespace WekezaCRM.Application.DTOs;

public record AccountDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public string AccountType { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Currency { get; init; } = "KES";
    public bool IsActive { get; init; }
}
