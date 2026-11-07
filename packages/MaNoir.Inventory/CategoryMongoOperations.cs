using MaNoir.Core.DataAccess;
using MaNoir.Inventory.Contracts.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class CategoryMongoOperations
{
    private readonly IMongoCollection<Category> _collection;

    public CategoryMongoOperations()
    {
        _collection = new MongoDbHelper().GetCollection<Category>();
    }

    public Task<Category> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.Find(category => category.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _collection.Find(category => true).ToListAsync(cancellationToken);
    }

    public Task SaveAsync(Category category, CancellationToken cancellationToken = default)
    {
        return _collection.ReplaceOneAsync(current => current.Id == category.Id, category, new ReplaceOptions() { IsUpsert = true }, cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _collection.DeleteOneAsync(category => category.Id == id, cancellationToken);
    }

    public Task<long> CountByParentIdAsync(string parentId, CancellationToken cancellationToken = default)
    {
        return _collection.CountDocumentsAsync(category => category.ParentId == parentId, cancellationToken: cancellationToken);
    }
}
