using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Application.DTOs;

public record InteractionDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public InteractionChannel Channel { get; init; }
    public string Subject { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime InteractionDate { get; init; }
    public int? DurationMinutes { get; init; }
}
