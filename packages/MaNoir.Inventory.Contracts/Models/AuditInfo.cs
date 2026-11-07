using System;

namespace MaNoir.Inventory.Contracts.Models;

// Minimal audit trail shared by every inventory entity.
public class AuditInfo
{
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }
}
