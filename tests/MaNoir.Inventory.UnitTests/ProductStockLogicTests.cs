using MaNoir.Inventory.Contracts.Models;
using System;

namespace MaNoir.Inventory.UnitTests;

[TestClass]
public sealed class ProductStockLogicTests
{
    [TestMethod]
    public void PrepareForSave_ShouldThrow_WhenProductIdIsMissing()
    {
        ProductStock stock = new ProductStock() { ProductId = " " };

        Assert.ThrowsExactly<ArgumentException>(() => { ProductStockLogic.PrepareForSave(stock); });
    }

    [TestMethod]
    public void PrepareForSave_ShouldSetDateAdded_WhenNew()
    {
        ProductStock stock = new ProductStock() { ProductId = "flour" };

        ProductStock prepared = ProductStockLogic.PrepareForSave(stock);

        Assert.AreNotEqual(default, prepared.DateAdded);
    }

    [TestMethod]
    public void PrepareForSave_ShouldNormalizeProductIdAndContainerId()
    {
        ProductStock stock = new ProductStock()
        {
            ProductId = " FLOUR ",
            Location = new ItemLocation() { ContainerId = " PANTRY-1 " }
        };

        ProductStock prepared = ProductStockLogic.PrepareForSave(stock);

        Assert.AreEqual("flour", prepared.ProductId);
        Assert.AreEqual("pantry-1", prepared.Location.ContainerId);
    }

    [TestMethod]
    public void ComputeDefaultExpiration_ShouldReturnNull_WhenProductIsNull()
    {
        DateTimeOffset? result = ProductStockLogic.ComputeDefaultExpiration(null, "box-1", DateTimeOffset.UtcNow);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ComputeDefaultExpiration_ShouldPreferPackagingDuration_OverProductDuration()
    {
        Product product = new Product()
        {
            Label = "Milk",
            StorageInfo = new ProductStorageInfo() { StandardExpirationDuration = 30 },
            Packagings = [new ProductPackaging() { Id = "box-1", StandardExpirationDuration = 7 }]
        };
        DateTimeOffset referenceDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        DateTimeOffset? result = ProductStockLogic.ComputeDefaultExpiration(product, "box-1", referenceDate);

        Assert.AreEqual(referenceDate.AddDays(7), result);
    }

    [TestMethod]
    public void ComputeDefaultExpiration_ShouldFallBackToProductDuration_WhenPackagingHasNone()
    {
        Product product = new Product()
        {
            Label = "Milk",
            StorageInfo = new ProductStorageInfo() { StandardExpirationDuration = 30 }
        };
        DateTimeOffset referenceDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        DateTimeOffset? result = ProductStockLogic.ComputeDefaultExpiration(product, null, referenceDate);

        Assert.AreEqual(referenceDate.AddDays(30), result);
    }

    [TestMethod]
    public void ComputeDefaultExpiration_ShouldReturnNull_WhenNoDurationIsConfigured()
    {
        Product product = new Product() { Label = "Salt" };

        DateTimeOffset? result = ProductStockLogic.ComputeDefaultExpiration(product, null, DateTimeOffset.UtcNow);

        Assert.IsNull(result);
    }
}
