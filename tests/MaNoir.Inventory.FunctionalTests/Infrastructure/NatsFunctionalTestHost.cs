using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory.FunctionalTests.Infrastructure;

internal sealed class NatsFunctionalTestHost : IAsyncDisposable
{
    private const ushort ContainerPort = 4222;
    private static readonly SemaphoreSlim Sync = new SemaphoreSlim(1, 1);
    private static readonly IContainer Container = new ContainerBuilder("nats:2.14.0")
        .WithPortBinding(ContainerPort, true)
        // nats:2.14.0 is a scratch image with no shell, so port probes that exec into the container never succeed.
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server is ready"))
        .Build();
    private static bool _started;

    public string Host => Container.Hostname;

    public int Port => Container.GetMappedPublicPort(ContainerPort);

    public string ConnectionString => $"nats://{Host}:{Port}";

    public async Task StartAsync()
    {
        await Sync.WaitAsync();
        try
        {
            if (_started)
                return;

            await Container.StartAsync();
            _started = true;
        }
        finally
        {
            Sync.Release();
        }
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
