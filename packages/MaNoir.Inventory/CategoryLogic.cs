using MaNoir.Inventory.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class CategoryLogic
{
    private readonly CategoryMongoOperations _mongoOperations = new();

    public Task<Category> GetByIdAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(categoryId);
        return normalizedId == null ? Task.FromResult<Category>(null) : _mongoOperations.GetByIdAsync(normalizedId, cancellationToken);
    }

    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _mongoOperations.GetAllAsync(cancellationToken);
    }

    public static Category PrepareForSave(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        if (string.IsNullOrWhiteSpace(category.Label))
            throw new ArgumentException("The category label cannot be empty.", nameof(category));

        bool isNew = string.IsNullOrWhiteSpace(category.Id);
        category.Id = isNew ? InventoryIdHelper.NewId() : InventoryIdHelper.NormalizeId(category.Id);
        category.ParentId = InventoryIdHelper.NormalizeId(category.ParentId);
        return category;
    }

    public async Task<Category> UpsertAsync(Category category, CancellationToken cancellationToken = default)
    {
        category = PrepareForSave(category);

        await _mongoOperations.SaveAsync(category, cancellationToken);
        return await _mongoOperations.GetByIdAsync(category.Id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(categoryId);
        if (normalizedId == null)
            return true;

        long childCount = await _mongoOperations.CountByParentIdAsync(normalizedId, cancellationToken);
        if (childCount > 0)
            return false;

        await _mongoOperations.DeleteAsync(normalizedId, cancellationToken);
        return true;
    }
}
