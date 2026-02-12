namespace WekezaCRM.Domain.Entities;

public class Campaign : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TargetSegment { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int TargetCustomers { get; set; }
    public int ReachedCustomers { get; set; }

    // Navigation properties
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
