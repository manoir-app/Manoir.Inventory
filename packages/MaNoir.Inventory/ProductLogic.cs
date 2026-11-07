using MaNoir.Inventory.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class ProductLogic
{
    private readonly ProductMongoOperations _mongoOperations = new();

    public Task<Product> GetByIdAsync(string productId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(productId);
        return normalizedId == null ? Task.FromResult<Product>(null) : _mongoOperations.GetByIdAsync(normalizedId, cancellationToken);
    }

    public Task<Product> GetByEanAsync(string ean, CancellationToken cancellationToken = default)
    {
        return string.IsNullOrWhiteSpace(ean) ? Task.FromResult<Product>(null) : _mongoOperations.GetByEanAsync(ean.Trim(), cancellationToken);
    }

    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _mongoOperations.GetAllAsync(cancellationToken);
    }

    public Task<List<Product>> GetByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        string normalizedCategoryId = InventoryIdHelper.NormalizeId(categoryId);
        return normalizedCategoryId == null ? Task.FromResult(new List<Product>()) : _mongoOperations.GetByCategoryAsync(normalizedCategoryId, cancellationToken);
    }

    public static Product PrepareForSave(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        if (string.IsNullOrWhiteSpace(product.Label))
            throw new ArgumentException("The product label cannot be empty.", nameof(product));

        bool isNew = string.IsNullOrWhiteSpace(product.Id);
        product.Id = isNew ? InventoryIdHelper.NewId() : InventoryIdHelper.NormalizeId(product.Id);

        if (string.IsNullOrEmpty(product.GenericName))
            product.GenericName = product.Label;

        foreach (ProductPackaging packaging in product.Packagings ?? [])
        {
            if (string.IsNullOrWhiteSpace(packaging.Id))
                packaging.Id = InventoryIdHelper.NewId();
            if (packaging.Ean == string.Empty)
                packaging.Ean = null;
        }

        product.Categories = [.. (product.Categories ?? []).Select(InventoryIdHelper.NormalizeId).Where(id => id != null).Distinct()];

        product.Audit ??= new AuditInfo();
        product.Audit.CreatedOn = isNew ? DateTimeOffset.UtcNow : product.Audit.CreatedOn;
        product.Audit.UpdatedOn = isNew ? null : DateTimeOffset.UtcNow;

        return product;
    }

    public async Task<Product> UpsertAsync(Product product, CancellationToken cancellationToken = default)
    {
        product = PrepareForSave(product);

        await _mongoOperations.SaveAsync(product, cancellationToken);
        return await _mongoOperations.GetByIdAsync(product.Id, cancellationToken);
    }

    public Task DeleteAsync(string productId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(productId);
        return normalizedId == null ? Task.CompletedTask : _mongoOperations.DeleteAsync(normalizedId, cancellationToken);
    }
}
