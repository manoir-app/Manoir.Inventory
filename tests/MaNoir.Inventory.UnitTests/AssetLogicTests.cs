using MaNoir.Inventory.Contracts.Models;
using System;

namespace MaNoir.Inventory.UnitTests;

[TestClass]
public sealed class AssetLogicTests
{
    [TestMethod]
    public void PrepareForSave_ShouldThrow_WhenLabelIsMissing()
    {
        Asset asset = new Asset() { Label = "" };

        Assert.ThrowsExactly<ArgumentException>(() => { AssetLogic.PrepareForSave(asset); });
    }

    [TestMethod]
    public void PrepareForSave_ShouldGenerateMaintenanceScheduleIds()
    {
        Asset asset = new Asset()
        {
            Label = "Boiler",
            MaintenanceSchedules = [new AssetMaintenanceSchedule() { Kind = "Descaling" }]
        };

        Asset prepared = AssetLogic.PrepareForSave(asset);

        Assert.IsFalse(string.IsNullOrWhiteSpace(prepared.MaintenanceSchedules[0].Id));
    }

    [TestMethod]
    public void PrepareForSave_ShouldNormalizeCategoryIdsAndContainerId()
    {
        Asset asset = new Asset()
        {
            Label = "Drill",
            CategoryIds = [" Tools ", "tools"],
            Location = new ItemLocation() { ContainerId = " GARAGE-SHELF " }
        };

        Asset prepared = AssetLogic.PrepareForSave(asset);

        CollectionAssert.AreEquivalent(new[] { "tools" }, prepared.CategoryIds);
        Assert.AreEqual("garage-shelf", prepared.Location.ContainerId);
    }
}
