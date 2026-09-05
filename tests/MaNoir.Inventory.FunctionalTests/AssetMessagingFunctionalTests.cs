using Home.Common.Messages;
using MaNoir.Inventory.Contracts.Models;
using MaNoir.Inventory.FunctionalTests.Infrastructure;
using NATS.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MaNoir.Inventory.FunctionalTests;

[TestClass]
[DoNotParallelize]
public sealed class AssetMessagingFunctionalTests
{
    [TestMethod]
    [TestCategory("Functional")]
    public async Task UpsertAsync_ShouldPublishMaintenanceDueMessage_WhenScheduleIsPastDue()
    {
        await using MongoDbFunctionalTestHost mongoHost = new MongoDbFunctionalTestHost();
        await mongoHost.StartAsync();
        await using NatsFunctionalTestHost natsHost = new NatsFunctionalTestHost();
        await natsHost.StartAsync();
        using ProcessEnvironmentVariableScope mongoScope = new ProcessEnvironmentVariableScope("MONGODB_CONNECTIONSTRING", mongoHost.ConnectionString);
        using ProcessEnvironmentVariableScope hostScope = new ProcessEnvironmentVariableScope("NATS_SERVICE_HOST", natsHost.Host);
        using ProcessEnvironmentVariableScope portScope = new ProcessEnvironmentVariableScope("NATS_SERVICE_PORT", natsHost.Port.ToString());
        using ProcessEnvironmentVariableScope compatPortScope = new ProcessEnvironmentVariableScope("NATS_PORT_4222_TCP_PROTO", null);

        ConnectionFactory factory = new ConnectionFactory();
        using IConnection connection = factory.CreateConnection(natsHost.ConnectionString);
        using ISyncSubscription maintenanceDueSubscription = connection.SubscribeSync(AssetMaintenanceDueMessage.TopicName);
        connection.Flush();

        AssetLogic logic = new AssetLogic();
        Asset created = await logic.UpsertAsync(new Asset()
        {
            Label = "Boiler",
            MaintenanceSchedules = [new AssetMaintenanceSchedule() { Kind = "Descaling", NextDueOn = DateTimeOffset.UtcNow.AddDays(-1) }]
        });

        Msg message = maintenanceDueSubscription.NextMessage(5000);
        AssetMaintenanceDueMessage payload = BaseMessage.ReadAs<AssetMaintenanceDueMessage>(Encoding.UTF8.GetString(message.Data));

        Assert.IsNotNull(payload);
        Assert.AreEqual(created.Id, payload.AssetId);
        Assert.AreEqual("Descaling", payload.Kind);
    }
}
