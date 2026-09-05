using MaNoir.Inventory.Contracts.Models;
using System;

namespace MaNoir.Inventory.UnitTests;

[TestClass]
public sealed class ProductLogicTests
{
    [TestMethod]
    public void PrepareForSave_ShouldThrow_WhenLabelIsMissing()
    {
        Product product = new Product() { Label = null };

        Assert.ThrowsExactly<ArgumentException>(() => { ProductLogic.PrepareForSave(product); });
    }

    [TestMethod]
    public void PrepareForSave_ShouldDefaultGenericNameToLabel()
    {
        Product product = new Product() { Label = "Flour" };

        Product prepared = ProductLogic.PrepareForSave(product);

        Assert.AreEqual("Flour", prepared.GenericName);
    }

    [TestMethod]
    public void PrepareForSave_ShouldGeneratePackagingIdsAndClearEmptyEan()
    {
        Product product = new Product()
        {
            Label = "Pasta",
            Packagings = [new ProductPackaging() { Label = "Box 500g", Ean = "" }]
        };

        Product prepared = ProductLogic.PrepareForSave(product);

        Assert.IsFalse(string.IsNullOrWhiteSpace(prepared.Packagings[0].Id));
        Assert.IsNull(prepared.Packagings[0].Ean);
    }

    [TestMethod]
    public void PrepareForSave_ShouldNormalizeAndDeduplicateCategories()
    {
        Product product = new Product()
        {
            Label = "Rice",
            Categories = [" Pantry ", "pantry", "GRAINS"]
        };

        Product prepared = ProductLogic.PrepareForSave(product);

        CollectionAssert.AreEquivalent(new[] { "pantry", "grains" }, prepared.Categories);
    }
}
