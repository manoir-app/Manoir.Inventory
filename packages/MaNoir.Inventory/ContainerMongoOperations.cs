using MaNoir.Core.DataAccess;
using MaNoir.Inventory.Contracts.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class ContainerMongoOperations
{
    private readonly IMongoCollection<Container> _collection;

    public ContainerMongoOperations()
    {
        _collection = new MongoDbHelper().GetCollection<Container>();
    }

    public Task<Container> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.Find(container => container.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Container>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _collection.Find(container => true).ToListAsync(cancellationToken);
    }

    public Task SaveAsync(Container container, CancellationToken cancellationToken = default)
    {
        return _collection.ReplaceOneAsync(current => current.Id == container.Id, container, new ReplaceOptions() { IsUpsert = true }, cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.DeleteOneAsync(container => container.Id == id, cancellationToken);
    }
}
