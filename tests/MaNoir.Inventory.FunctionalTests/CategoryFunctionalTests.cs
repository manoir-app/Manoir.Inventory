using MaNoir.Inventory.Contracts.Models;
using MaNoir.Inventory.FunctionalTests.Infrastructure;
using System.Threading.Tasks;

namespace MaNoir.Inventory.FunctionalTests;

[TestClass]
[DoNotParallelize]
public sealed class CategoryFunctionalTests
{
    [TestMethod]
    [TestCategory("Functional")]
    public async Task UpsertAsync_ShouldPersistAndReturnCategory()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        CategoryLogic logic = new CategoryLogic();
        Category created = await logic.UpsertAsync(new Category() { Label = "Pantry" });
        Category reloaded = await logic.GetByIdAsync(created.Id);

        Assert.IsNotNull(reloaded);
        Assert.AreEqual("Pantry", reloaded.Label);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task DeleteAsync_ShouldBeRefused_WhenChildCategoriesExist()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        CategoryLogic logic = new CategoryLogic();
        Category parent = await logic.UpsertAsync(new Category() { Label = "Kitchen" });
        await logic.UpsertAsync(new Category() { Label = "Cookware", ParentId = parent.Id });

        bool deleted = await logic.DeleteAsync(parent.Id);
        Category stillThere = await logic.GetByIdAsync(parent.Id);

        Assert.IsFalse(deleted);
        Assert.IsNotNull(stillThere);
    }
}
