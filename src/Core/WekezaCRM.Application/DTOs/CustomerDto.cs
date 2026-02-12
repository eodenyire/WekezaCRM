using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Application.DTOs;

public record CustomerDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public DateTime? DateOfBirth { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public CustomerSegment Segment { get; init; }
    public KYCStatus KYCStatus { get; init; }
    public string? CustomerReference { get; init; }
    public decimal? CreditScore { get; init; }
    public decimal? LifetimeValue { get; init; }
    public int? RiskScore { get; init; }
    public bool IsActive { get; init; }
}
