using MaNoir.Core.DataAccess;
using MaNoir.Inventory.Contracts.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class AssetMongoOperations
{
    private readonly IMongoCollection<Asset> _collection;

    public AssetMongoOperations()
    {
        _collection = new MongoDbHelper().GetCollection<Asset>();
    }

    public Task<Asset> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.Find(asset => asset.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Asset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _collection.Find(asset => true).ToListAsync(cancellationToken);
    }

    public Task<List<Asset>> GetByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Asset> filter = Builders<Asset>.Filter.AnyEq(asset => asset.CategoryIds, categoryId);
        return _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public Task<List<Asset>> GetWithMaintenanceDueAsync(System.DateTimeOffset upToDate, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Asset> filter = Builders<Asset>.Filter.ElemMatch(
            asset => asset.MaintenanceSchedules,
            schedule => schedule.NextDueOn != null && schedule.NextDueOn <= upToDate);
        return _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public Task SaveAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        return _collection.ReplaceOneAsync(current => current.Id == asset.Id, asset, new ReplaceOptions() { IsUpsert = true }, cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.DeleteOneAsync(asset => asset.Id == id, cancellationToken);
    }
}
