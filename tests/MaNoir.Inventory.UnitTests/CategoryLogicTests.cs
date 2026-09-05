using MaNoir.Inventory.Contracts.Models;
using System;

namespace MaNoir.Inventory.UnitTests;

[TestClass]
public sealed class CategoryLogicTests
{
    [TestMethod]
    public void PrepareForSave_ShouldThrow_WhenLabelIsMissing()
    {
        Category category = new Category() { Label = "  " };

        Assert.ThrowsExactly<ArgumentException>(() => { CategoryLogic.PrepareForSave(category); });
    }

    [TestMethod]
    public void PrepareForSave_ShouldThrow_WhenCategoryIsNull()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => { CategoryLogic.PrepareForSave(null); });
    }

    [TestMethod]
    public void PrepareForSave_ShouldGenerateId_WhenNew()
    {
        Category category = new Category() { Label = "Pantry" };

        Category prepared = CategoryLogic.PrepareForSave(category);

        Assert.IsFalse(string.IsNullOrWhiteSpace(prepared.Id));
        Assert.IsTrue(Guid.TryParse(prepared.Id, out _));
    }

    [TestMethod]
    public void PrepareForSave_ShouldNormalizeExistingIdAndParentId()
    {
        Category category = new Category() { Id = " ABC-123 ", Label = "Tools", ParentId = " PARENT-1 " };

        Category prepared = CategoryLogic.PrepareForSave(category);

        Assert.AreEqual("abc-123", prepared.Id);
        Assert.AreEqual("parent-1", prepared.ParentId);
    }
}
