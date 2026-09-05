using MaNoir.Inventory.Contracts.Models;
using MaNoir.Inventory.FunctionalTests.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaNoir.Inventory.FunctionalTests;

[TestClass]
[DoNotParallelize]
public sealed class ProductFunctionalTests
{
    [TestMethod]
    [TestCategory("Functional")]
    public async Task UpsertAsync_ShouldPersistProductAndBeFindableByEan()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        ProductLogic logic = new ProductLogic();
        await logic.UpsertAsync(new Product()
        {
            Label = "Flour T55",
            Packagings = [new ProductPackaging() { Label = "1kg bag", Ean = "3017620425035" }]
        });

        Product foundByEan = await logic.GetByEanAsync("3017620425035");

        Assert.IsNotNull(foundByEan);
        Assert.AreEqual("Flour T55", foundByEan.Label);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetByCategoryAsync_ShouldReturnOnlyMatchingProducts()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        ProductLogic logic = new ProductLogic();
        Product matching = await logic.UpsertAsync(new Product() { Label = "Pasta", Categories = ["pantry"] });
        await logic.UpsertAsync(new Product() { Label = "Soap", Categories = ["housekeeping"] });

        List<Product> results = await logic.GetByCategoryAsync("pantry");

        Assert.HasCount(1, results);
        Assert.AreEqual(matching.Id, results[0].Id);
    }
}
