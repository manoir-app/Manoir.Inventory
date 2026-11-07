using Home.Common;
using Home.Common.Messages;
using MaNoir.Inventory.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class ProductStockLogic
{
    // How many days ahead of DateExpiration an inventory.stock.expiring event is raised.
    public const int ExpiringSoonThresholdDays = 3;

    private readonly ProductStockMongoOperations _mongoOperations = new();

    public Task<ProductStock> GetByIdAsync(string stockId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(stockId);
        return normalizedId == null ? Task.FromResult<ProductStock>(null) : _mongoOperations.GetByIdAsync(normalizedId, cancellationToken);
    }

    public Task<List<ProductStock>> GetByProductIdAsync(string productId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(productId);
        return normalizedId == null ? Task.FromResult(new List<ProductStock>()) : _mongoOperations.GetByProductIdAsync(normalizedId, cancellationToken);
    }

    public Task<List<ProductStock>> GetByContainerIdAsync(string containerId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(containerId);
        return normalizedId == null ? Task.FromResult(new List<ProductStock>()) : _mongoOperations.GetByContainerIdAsync(normalizedId, cancellationToken);
    }

    // Items whose combined new/opened quantity dropped to zero or below; consumers compare against their own threshold.
    public Task<List<ProductStock>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return _mongoOperations.GetLowStockAsync(cancellationToken);
    }

    public Task<List<ProductStock>> GetExpiringAsync(int withinDays = 0, CancellationToken cancellationToken = default)
    {
        DateTimeOffset upToDate = DateTimeOffset.UtcNow.AddDays(withinDays);
        return _mongoOperations.GetExpiringAsync(upToDate, cancellationToken);
    }

    // Packaging-level duration wins over the product-level default; no duration found means no computed expiration.
    public static DateTimeOffset? ComputeDefaultExpiration(Product product, string packagingId, DateTimeOffset referenceDate)
    {
        if (product == null)
            return null;

        int? durationDays = null;
        if (!string.IsNullOrWhiteSpace(packagingId) && product.Packagings != null)
            durationDays = product.Packagings.FirstOrDefault(packaging => packaging.Id == packagingId)?.StandardExpirationDuration;

        durationDays ??= product.StorageInfo?.StandardExpirationDuration;

        return durationDays.HasValue ? referenceDate.AddDays(durationDays.Value) : null;
    }

    public static ProductStock PrepareForSave(ProductStock stock)
    {
        ArgumentNullException.ThrowIfNull(stock);
        if (string.IsNullOrWhiteSpace(stock.ProductId))
            throw new ArgumentException("The product identifier cannot be empty.", nameof(stock));

        bool isNew = string.IsNullOrWhiteSpace(stock.Id);
        stock.Id = isNew ? InventoryIdHelper.NewId() : InventoryIdHelper.NormalizeId(stock.Id);
        stock.ProductId = InventoryIdHelper.NormalizeId(stock.ProductId);

        stock.Location ??= new ItemLocation();
        stock.Location.ContainerId = InventoryIdHelper.NormalizeId(stock.Location.ContainerId);

        if (isNew && stock.DateAdded == default)
            stock.DateAdded = DateTimeOffset.UtcNow;

        stock.Audit ??= new AuditInfo();
        stock.Audit.CreatedOn = isNew ? DateTimeOffset.UtcNow : stock.Audit.CreatedOn;
        stock.Audit.UpdatedOn = isNew ? null : DateTimeOffset.UtcNow;

        return stock;
    }

    public async Task<ProductStock> UpsertAsync(ProductStock stock, CancellationToken cancellationToken = default)
    {
        stock = PrepareForSave(stock);

        if (stock.DateExpiration == null)
        {
            Product product = await new ProductLogic().GetByIdAsync(stock.ProductId, cancellationToken);
            stock.DateExpiration = ComputeDefaultExpiration(product, stock.PackagingId, stock.DateAdded);
        }

        await _mongoOperations.SaveAsync(stock, cancellationToken);
        ProductStock savedStock = await _mongoOperations.GetByIdAsync(stock.Id, cancellationToken);
        PublishChangeEventsBestEffort(savedStock);
        return savedStock;
    }

    private static void PublishChangeEventsBestEffort(ProductStock stock)
    {
        if (stock == null)
            return;

        try
        {
            NatsInterprocess.Push(new StockChangedMessage()
            {
                StockId = stock.Id,
                ProductId = stock.ProductId,
                QuantityNew = stock.QuantityNew,
                QuantityOpened = stock.QuantityOpened
            });

            if (stock.QuantityNew + stock.QuantityOpened <= 0)
            {
                NatsInterprocess.Push(new StockLowMessage()
                {
                    StockId = stock.Id,
                    ProductId = stock.ProductId,
                    QuantityNew = stock.QuantityNew,
                    QuantityOpened = stock.QuantityOpened
                });
            }

            if (stock.DateExpiration.HasValue && stock.DateExpiration.Value <= DateTimeOffset.UtcNow.AddDays(ExpiringSoonThresholdDays))
            {
                NatsInterprocess.Push(new StockExpiringMessage()
                {
                    StockId = stock.Id,
                    ProductId = stock.ProductId,
                    DateExpiration = stock.DateExpiration.Value
                });
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Unable to publish stock change events for {stock.Id}: {exception.Message}");
        }
    }

    public Task DeleteAsync(string stockId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(stockId);
        return normalizedId == null ? Task.CompletedTask : _mongoOperations.DeleteAsync(normalizedId, cancellationToken);
    }
}
