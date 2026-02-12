namespace WekezaCRM.Domain.Entities;

public class CaseNote : BaseEntity
{
    public Guid CaseId { get; set; }
    public string Note { get; set; } = string.Empty;
    public bool IsInternal { get; set; }

    // Navigation properties
    public Case Case { get; set; } = null!;
}
