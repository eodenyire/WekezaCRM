using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Entities;

public class Interaction : BaseEntity
{
    public Guid CustomerId { get; set; }
    public InteractionChannel Channel { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime InteractionDate { get; set; }
    public int? DurationMinutes { get; set; }
    public Guid? UserId { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
}
