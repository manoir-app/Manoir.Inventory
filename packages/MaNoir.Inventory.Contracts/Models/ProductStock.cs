using System;
using System.Collections.Generic;

namespace MaNoir.Inventory.Contracts.Models;

public enum ProductReservationKind
{
    ForRecipe,
    ForPlannedMeal
}

public class ReservedProduct
{
    public ProductReservationKind Kind { get; set; }
    public string ReservationId { get; set; }
    public string AssociatedId { get; set; }
}

// A physical quantity of a Product sitting in a Container.
public class ProductStock
{
    public ProductStock()
    {
        Reservations = new List<ReservedProduct>();
        Location = new ItemLocation();
        Audit = new AuditInfo();
    }

    public string Id { get; set; }
    public string ProductId { get; set; }
    public ProductMetaType ProductMetaType { get; set; }
    public bool IsMainStorage { get; set; }

    public ItemLocation Location { get; set; }

    public DateTimeOffset DateAdded { get; set; }
    public DateTimeOffset? DateExpiration { get; set; }

    public decimal QuantityNew { get; set; }
    public decimal QuantityOpened { get; set; }
    public decimal OriginalQuantity { get; set; }
    public string UnitId { get; set; }

    public string PackagingId { get; set; }
    public ProductPackagingType PackagingType { get; set; }

    public Dictionary<string, string> AssociatedTags { get; set; }

    public List<ReservedProduct> Reservations { get; set; }

    public AuditInfo Audit { get; set; }
}
