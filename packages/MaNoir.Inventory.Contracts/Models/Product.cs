using System;
using System.Collections.Generic;

namespace MaNoir.Inventory.Contracts.Models;

[Flags]
public enum ProductMetaType
{
    Consumable = 1,
    Perishable = 2,
    Food = 16,
    Beverage = 32,
    HouseWare = 128,
    HouseKeeping = 64
}

public enum ProductOrigin
{
    Unknown,
    Bought,
    SelfMade
}

public enum ProductPackagingType
{
    Box,
    Bottle
}

public enum UnitMetaType
{
    Count,
    Weight,
    Volume
}

public class ProductType
{
    public string Id { get; set; }
    public string Label { get; set; }
    public ProductMetaType MetaType { get; set; }
    public Dictionary<string, ProductImages> Images { get; set; }
}

// Catalog reference for a consumable good (pantry, household products).
public class Product
{
    public Product()
    {
        Categories = new List<string>();
        Images = new Dictionary<string, ProductImages>();
        Packagings = new List<ProductPackaging>();
        StorageInfo = new ProductStorageInfo();
        Audit = new AuditInfo();
    }

    public string Id { get; set; }
    public string Label { get; set; }
    public string GenericName { get; set; }
    public ProductMetaType MetaType { get; set; }
    public string ProductTypeId { get; set; }
    public Dictionary<string, ProductImages> Images { get; set; }

    public int? CommonDuration { get; set; }

    public UnitMetaType UnitMetaType { get; set; }
    public string UnitId { get; set; }
    public string UnitExplanation { get; set; }

    // References Category ids scoped to CategoryScope.Product.
    public List<string> Categories { get; set; }

    public List<ProductPackaging> Packagings { get; set; }

    public ProductStorageInfo StorageInfo { get; set; }

    public ProductConsumption ConsumptionInfo { get; set; }

    public AuditInfo Audit { get; set; }
}

public class ProductConsumption
{
    public int? AutoDecrementAmount { get; set; }
    public TimeSpan? AutoDecrementPeriod { get; set; }
    public DateTimeOffset? LastAutoDecrementDate { get; set; }
}

public class ProductStorageInfo
{
    public int? StandardExpirationDuration { get; set; }
    public int? StandardExpirationTolerance { get; set; }
    public string DefaultContainerIdWhenNew { get; set; }
    public string DefaultContainerSubElementIdWhenNew { get; set; }
    public string DefaultStorageCommentWhenNew { get; set; }
    public string DefaultContainerIdWhenOpened { get; set; }
    public string DefaultContainerSubElementIdWhenOpened { get; set; }
    public string DefaultStorageCommentWhenOpened { get; set; }
}

public class ProductImages
{
    public string[] BigImageUrls { get; set; }
    public string[] SmallImageUrls { get; set; }
    public string[] IconUrls { get; set; }
}

public class ProductPackaging
{
    public string Id { get; set; }
    public string Label { get; set; }

    // Fixed: was untyped `int` in the legacy model.
    public ProductPackagingType PackagingType { get; set; }

    public ProductOrigin OriginKind { get; set; }
    public string OriginId { get; set; }

    public string Ean { get; set; }
    public decimal? PackQuantity { get; set; }
    public Dictionary<string, ProductImages> Images { get; set; }

    public int? StandardExpirationDuration { get; set; }
}
