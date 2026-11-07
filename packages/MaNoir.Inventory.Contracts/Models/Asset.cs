using System;
using System.Collections.Generic;

namespace MaNoir.Inventory.Contracts.Models;

public enum AssetKind
{
    Appliance,
    Tool,
    Book,
    Vehicle,
    Other
}

public enum MaintenanceFrequencyUnit
{
    Days,
    Weeks,
    Months,
    Years
}

// A non-consumable possession: equipment, tool, book, or other durable good.
public class Asset
{
    public Asset()
    {
        CategoryIds = new List<string>();
        PhotoUrls = new List<string>();
        MaintenanceSchedules = new List<AssetMaintenanceSchedule>();
        Tags = new List<string>();
        Location = new ItemLocation();
        Audit = new AuditInfo();
        PrivacyLevel = PrivacyLevel.Household;
    }

    public string Id { get; set; }
    public string Label { get; set; }
    public string Description { get; set; }
    public AssetKind Kind { get; set; }

    // References Category ids scoped to CategoryScope.Asset.
    public List<string> CategoryIds { get; set; }

    public string Brand { get; set; }
    public string Model { get; set; }
    public string Variant { get; set; }
    public string SerialNumber { get; set; }

    public ItemLocation Location { get; set; }

    public List<string> PhotoUrls { get; set; }

    public AssetPurchaseInfo Purchase { get; set; }
    public AssetWarrantyInfo Warranty { get; set; }
    public List<AssetMaintenanceSchedule> MaintenanceSchedules { get; set; }

    public List<string> Tags { get; set; }
    public PrivacyLevel PrivacyLevel { get; set; }

    public AuditInfo Audit { get; set; }
}

public class AssetPurchaseInfo
{
    public DateTimeOffset? Date { get; set; }
    public decimal? Price { get; set; }
    public string Vendor { get; set; }
    public string ReceiptUrl { get; set; }
}

public class AssetWarrantyInfo
{
    public DateTimeOffset? EndsOn { get; set; }
    public string Coverage { get; set; }
    public string ManualUrl { get; set; }
}

public class AssetMaintenanceSchedule
{
    public string Id { get; set; }
    public string Kind { get; set; }
    public int FrequencyValue { get; set; }
    public MaintenanceFrequencyUnit FrequencyUnit { get; set; }
    public DateTimeOffset? NextDueOn { get; set; }
    public DateTimeOffset? LastDoneOn { get; set; }
}
