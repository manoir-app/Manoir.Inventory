using MaNoir.Inventory.Contracts.Models;
using MaNoir.Inventory.FunctionalTests.Infrastructure;
using System.Threading.Tasks;

namespace MaNoir.Inventory.FunctionalTests;

[TestClass]
[DoNotParallelize]
public sealed class ContainerFunctionalTests
{
    [TestMethod]
    [TestCategory("Functional")]
    public async Task UpsertAsync_ShouldPersistContainerWithSubElements()
    {
        await using MongoDbFunctionalTestHost host = new MongoDbFunctionalTestHost();
        await host.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", host.ConnectionString);

        ContainerLogic logic = new ContainerLogic();
        Container created = await logic.UpsertAsync(new Container()
        {
            Label = "Kitchen cupboard",
            ZoneId = "ground-floor",
            RoomId = "kitchen",
            SubElements = [new ContainerSubElement() { Label = "Top shelf" }]
        });

        Container reloaded = await logic.GetByIdAsync(created.Id);

        Assert.IsNotNull(reloaded);
        Assert.AreEqual("ground-floor", reloaded.ZoneId);
        Assert.HasCount(1, reloaded.SubElements);
        Assert.IsFalse(string.IsNullOrWhiteSpace(reloaded.SubElements[0].Id));
    }
}
