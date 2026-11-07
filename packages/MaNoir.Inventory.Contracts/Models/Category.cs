using System;

namespace MaNoir.Inventory.Contracts.Models;

// Shared classification, reusable by products and assets alike.
[Flags]
public enum CategoryScope
{
    Product = 1,
    Asset = 2
}

public class Category
{
    public string Id { get; set; }
    public string Label { get; set; }
    public string ParentId { get; set; }
    public string IconId { get; set; }
    public CategoryScope Scope { get; set; } = CategoryScope.Product | CategoryScope.Asset;
}
