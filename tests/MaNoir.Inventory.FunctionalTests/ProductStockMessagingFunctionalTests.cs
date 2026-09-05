using Home.Common.Messages;
using MaNoir.Inventory.Contracts.Models;
using MaNoir.Inventory.FunctionalTests.Infrastructure;
using NATS.Client;
using System.Text;
using System.Threading.Tasks;

namespace MaNoir.Inventory.FunctionalTests;

[TestClass]
[DoNotParallelize]
public sealed class ProductStockMessagingFunctionalTests
{
    [TestMethod]
    [TestCategory("Functional")]
    public async Task UpsertAsync_ShouldPublishStockLowMessage_WhenQuantityReachesZero()
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
        using ISyncSubscription changedSubscription = connection.SubscribeSync(StockChangedMessage.TopicName);
        using ISyncSubscription lowStockSubscription = connection.SubscribeSync(StockLowMessage.TopicName);
        connection.Flush();

        ProductStockLogic logic = new ProductStockLogic();
        ProductStock created = await logic.UpsertAsync(new ProductStock() { ProductId = "flour", QuantityNew = 0, QuantityOpened = 0 });

        Msg changedMessage = changedSubscription.NextMessage(5000);
        Msg lowStockMessage = lowStockSubscription.NextMessage(5000);
        StockChangedMessage changedPayload = BaseMessage.ReadAs<StockChangedMessage>(Encoding.UTF8.GetString(changedMessage.Data));
        StockLowMessage lowStockPayload = BaseMessage.ReadAs<StockLowMessage>(Encoding.UTF8.GetString(lowStockMessage.Data));

        Assert.IsNotNull(changedPayload);
        Assert.AreEqual(created.Id, changedPayload.StockId);
        Assert.IsNotNull(lowStockPayload);
        Assert.AreEqual(created.Id, lowStockPayload.StockId);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task UpsertAsync_ShouldNotPublishStockLowMessage_WhenQuantityIsPositive()
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
        using ISyncSubscription lowStockSubscription = connection.SubscribeSync(StockLowMessage.TopicName);
        connection.Flush();

        ProductStockLogic logic = new ProductStockLogic();
        await logic.UpsertAsync(new ProductStock() { ProductId = "rice", QuantityNew = 2 });

        // NextMessage throws on timeout instead of returning null; no low-stock message is expected here.
        Assert.ThrowsExactly<NATSTimeoutException>(() => { lowStockSubscription.NextMessage(1000); });
    }
}
