using MaNoir.Inventory.Contracts.Models;
using System;

namespace MaNoir.Inventory.UnitTests;

[TestClass]
public sealed class ContainerLogicTests
{
    [TestMethod]
    public void PrepareForSave_ShouldThrow_WhenLabelIsMissing()
    {
        Container container = new Container() { Label = "" };

        Assert.ThrowsExactly<ArgumentException>(() => { ContainerLogic.PrepareForSave(container); });
    }

    [TestMethod]
    public void PrepareForSave_ShouldStampCreatedOn_WhenNew()
    {
        Container container = new Container() { Label = "Fridge" };

        Container prepared = ContainerLogic.PrepareForSave(container);

        Assert.AreNotEqual(default, prepared.Audit.CreatedOn);
        Assert.IsNull(prepared.Audit.UpdatedOn);
    }

    [TestMethod]
    public void PrepareForSave_ShouldStampUpdatedOn_WhenExisting()
    {
        Container container = new Container() { Id = "existing-id", Label = "Freezer" };
        container.Audit.CreatedOn = DateTimeOffset.UtcNow.AddDays(-10);

        Container prepared = ContainerLogic.PrepareForSave(container);

        Assert.IsNotNull(prepared.Audit.UpdatedOn);
        Assert.AreEqual("existing-id", prepared.Id);
    }

    [TestMethod]
    public void PrepareForSave_ShouldGenerateSubElementIds()
    {
        Container container = new Container()
        {
            Label = "Cupboard",
            SubElements = [new ContainerSubElement() { Label = "Shelf 1" }]
        };

        Container prepared = ContainerLogic.PrepareForSave(container);

        Assert.IsFalse(string.IsNullOrWhiteSpace(prepared.SubElements[0].Id));
    }
}
