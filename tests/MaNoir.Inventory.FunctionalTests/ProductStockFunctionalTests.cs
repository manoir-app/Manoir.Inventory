using MaNoir.Inventory.Contracts.Models;
using MaNoir.Inventory.FunctionalTests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaNoir.Inventory.FunctionalTests;

[TestClass]
[DoNotParallelize]
public sealed class ProductStockFunctionalTests
{
    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetLowStockAsync_ShouldReturnOnlyDepletedStocks()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        ProductStockLogic logic = new ProductStockLogic();
        ProductStock depleted = await logic.UpsertAsync(new ProductStock() { ProductId = "flour", QuantityNew = 0, QuantityOpened = 0 });
        await logic.UpsertAsync(new ProductStock() { ProductId = "rice", QuantityNew = 2 });

        List<ProductStock> lowStocks = await logic.GetLowStockAsync();

        Assert.HasCount(1, lowStocks);
        Assert.AreEqual(depleted.Id, lowStocks[0].Id);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetByContainerIdAsync_ShouldReturnStocksInThatContainer()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        ProductStockLogic logic = new ProductStockLogic();
        ProductStock inPantry = await logic.UpsertAsync(new ProductStock()
        {
            ProductId = "flour",
            Location = new ItemLocation() { ContainerId = "pantry-shelf" }
        });
        await logic.UpsertAsync(new ProductStock()
        {
            ProductId = "detergent",
            Location = new ItemLocation() { ContainerId = "utility-cupboard" }
        });

        List<ProductStock> results = await logic.GetByContainerIdAsync("pantry-shelf");

        Assert.HasCount(1, results);
        Assert.AreEqual(inPantry.Id, results[0].Id);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task UpsertAsync_ShouldComputeDefaultExpiration_FromProductStorageInfo()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        Product product = await new ProductLogic().UpsertAsync(new Product()
        {
            Label = "Yogurt",
            StorageInfo = new ProductStorageInfo() { StandardExpirationDuration = 10 }
        });

        ProductStockLogic logic = new ProductStockLogic();
        ProductStock created = await logic.UpsertAsync(new ProductStock() { ProductId = product.Id });

        Assert.IsNotNull(created.DateExpiration);
        Assert.AreEqual(created.DateAdded.AddDays(10).Date, created.DateExpiration.Value.Date);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetExpiringAsync_ShouldReturnOnlyStocksExpiringWithinRange()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        ProductStockLogic logic = new ProductStockLogic();
        ProductStock expiringSoon = await logic.UpsertAsync(new ProductStock()
        {
            ProductId = "yogurt",
            DateExpiration = DateTimeOffset.UtcNow.AddDays(2)
        });
        await logic.UpsertAsync(new ProductStock()
        {
            ProductId = "canned-beans",
            DateExpiration = DateTimeOffset.UtcNow.AddYears(1)
        });

        List<ProductStock> results = await logic.GetExpiringAsync(withinDays: 3);

        Assert.HasCount(1, results);
        Assert.AreEqual(expiringSoon.Id, results[0].Id);
    }
}
