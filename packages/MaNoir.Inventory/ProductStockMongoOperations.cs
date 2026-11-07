using MaNoir.Core.DataAccess;
using MaNoir.Inventory.Contracts.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class ProductStockMongoOperations
{
    private readonly IMongoCollection<ProductStock> _collection;

    public ProductStockMongoOperations()
    {
        _collection = new MongoDbHelper().GetCollection<ProductStock>();
    }

    public Task<ProductStock> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.Find(stock => stock.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<ProductStock>> GetByProductIdAsync(string productId, CancellationToken cancellationToken = default)
    {
        return _collection.Find(stock => stock.ProductId == productId).ToListAsync(cancellationToken);
    }

    public Task<List<ProductStock>> GetByContainerIdAsync(string containerId, CancellationToken cancellationToken = default)
    {
        return _collection.Find(stock => stock.Location.ContainerId == containerId).ToListAsync(cancellationToken);
    }

    public Task<List<ProductStock>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return _collection.Find(stock => stock.QuantityNew + stock.QuantityOpened <= 0).ToListAsync(cancellationToken);
    }

    public Task<List<ProductStock>> GetExpiringAsync(DateTimeOffset upToDate, CancellationToken cancellationToken = default)
    {
        return _collection.Find(stock => stock.DateExpiration != null && stock.DateExpiration <= upToDate).ToListAsync(cancellationToken);
    }

    public Task SaveAsync(ProductStock stock, CancellationToken cancellationToken = default)
    {
        return _collection.ReplaceOneAsync(current => current.Id == stock.Id, stock, new ReplaceOptions() { IsUpsert = true }, cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.DeleteOneAsync(stock => stock.Id == id, cancellationToken);
    }
}
