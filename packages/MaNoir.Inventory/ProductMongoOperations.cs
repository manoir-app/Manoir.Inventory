using MaNoir.Core.DataAccess;
using MaNoir.Inventory.Contracts.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class ProductMongoOperations
{
    private readonly IMongoCollection<Product> _collection;

    public ProductMongoOperations()
    {
        _collection = new MongoDbHelper().GetCollection<Product>();
    }

    public Task<Product> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.Find(product => product.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Product> GetByEanAsync(string ean, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq("Packagings.Ean", ean);
        return _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _collection.Find(product => true).ToListAsync(cancellationToken);
    }

    public Task<List<Product>> GetByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.AnyEq(product => product.Categories, categoryId);
        return _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public Task SaveAsync(Product product, CancellationToken cancellationToken = default)
    {
        return _collection.ReplaceOneAsync(current => current.Id == product.Id, product, new ReplaceOptions() { IsUpsert = true }, cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.DeleteOneAsync(product => product.Id == id, cancellationToken);
    }
}
