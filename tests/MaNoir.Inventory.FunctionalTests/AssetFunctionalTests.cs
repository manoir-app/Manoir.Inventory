using MaNoir.Inventory.Contracts.Models;
using MaNoir.Inventory.FunctionalTests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaNoir.Inventory.FunctionalTests;

[TestClass]
[DoNotParallelize]
public sealed class AssetFunctionalTests
{
    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetWithMaintenanceDueAsync_ShouldReturnOnlyAssetsWithADueSchedule()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        AssetLogic logic = new AssetLogic();
        Asset dueAsset = await logic.UpsertAsync(new Asset()
        {
            Label = "Boiler",
            MaintenanceSchedules = [new AssetMaintenanceSchedule() { Kind = "Descaling", NextDueOn = DateTimeOffset.UtcNow.AddDays(-1) }]
        });
        await logic.UpsertAsync(new Asset()
        {
            Label = "Drill",
            MaintenanceSchedules = [new AssetMaintenanceSchedule() { Kind = "Check", NextDueOn = DateTimeOffset.UtcNow.AddYears(1) }]
        });

        List<Asset> results = await logic.GetWithMaintenanceDueAsync();

        Assert.HasCount(1, results);
        Assert.AreEqual(dueAsset.Id, results[0].Id);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task UpsertAsync_ShouldPersistWarrantyAndSerialNumber()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        AssetLogic logic = new AssetLogic();
        Asset created = await logic.UpsertAsync(new Asset()
        {
            Label = "Washing machine",
            SerialNumber = "WM-12345",
            Warranty = new AssetWarrantyInfo() { EndsOn = DateTimeOffset.UtcNow.AddYears(2) }
        });

        Asset reloaded = await logic.GetByIdAsync(created.Id);

        Assert.IsNotNull(reloaded);
        Assert.AreEqual("WM-12345", reloaded.SerialNumber);
        Assert.IsNotNull(reloaded.Warranty?.EndsOn);
    }
}
