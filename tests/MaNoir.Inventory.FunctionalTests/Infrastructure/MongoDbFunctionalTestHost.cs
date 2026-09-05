using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Testcontainers.MongoDb;

namespace MaNoir.Inventory.FunctionalTests.Infrastructure;

// Shared Mongo container across the assembly test run; each test drops the database before use.
internal sealed class MongoDbFunctionalTestHost : IAsyncDisposable
{
    private static readonly SemaphoreSlim Sync = new SemaphoreSlim(1, 1);
    private static readonly MongoDbContainer Container = new MongoDbBuilder("mongo:7.0")
        .Build();
    private static bool _started;

    public string ConnectionString
    {
        get { return Container.GetConnectionString(); }
    }

    public async Task StartAsync()
    {
        await Sync.WaitAsync();
        try
        {
            if (!_started)
            {
                await Container.StartAsync();
                _started = true;
            }

            MongoClient client = new MongoClient(ConnectionString);
            await client.DropDatabaseAsync(MaNoir.Core.DataAccess.MongoDbHelper.DefaultDatabaseName);
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
