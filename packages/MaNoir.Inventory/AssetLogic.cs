using Home.Common;
using Home.Common.Messages;
using MaNoir.Inventory.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class AssetLogic
{
    private readonly AssetMongoOperations _mongoOperations = new();

    public Task<Asset> GetByIdAsync(string assetId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(assetId);
        return normalizedId == null ? Task.FromResult<Asset>(null) : _mongoOperations.GetByIdAsync(normalizedId, cancellationToken);
    }

    public Task<List<Asset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _mongoOperations.GetAllAsync(cancellationToken);
    }

    public Task<List<Asset>> GetByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        string normalizedCategoryId = InventoryIdHelper.NormalizeId(categoryId);
        return normalizedCategoryId == null ? Task.FromResult(new List<Asset>()) : _mongoOperations.GetByCategoryAsync(normalizedCategoryId, cancellationToken);
    }

    public Task<List<Asset>> GetWithMaintenanceDueAsync(int withinDays = 0, CancellationToken cancellationToken = default)
    {
        DateTimeOffset upToDate = DateTimeOffset.UtcNow.AddDays(withinDays);
        return _mongoOperations.GetWithMaintenanceDueAsync(upToDate, cancellationToken);
    }

    public static Asset PrepareForSave(Asset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);
        if (string.IsNullOrWhiteSpace(asset.Label))
            throw new ArgumentException("The asset label cannot be empty.", nameof(asset));

        bool isNew = string.IsNullOrWhiteSpace(asset.Id);
        asset.Id = isNew ? InventoryIdHelper.NewId() : InventoryIdHelper.NormalizeId(asset.Id);

        asset.CategoryIds = [.. (asset.CategoryIds ?? []).Select(InventoryIdHelper.NormalizeId).Where(id => id != null).Distinct()];

        asset.Location ??= new ItemLocation();
        asset.Location.ContainerId = InventoryIdHelper.NormalizeId(asset.Location.ContainerId);

        foreach (AssetMaintenanceSchedule schedule in asset.MaintenanceSchedules ?? [])
        {
            if (string.IsNullOrWhiteSpace(schedule.Id))
                schedule.Id = InventoryIdHelper.NewId();
        }

        asset.Audit ??= new AuditInfo();
        asset.Audit.CreatedOn = isNew ? DateTimeOffset.UtcNow : asset.Audit.CreatedOn;
        asset.Audit.UpdatedOn = isNew ? null : DateTimeOffset.UtcNow;

        return asset;
    }

    public async Task<Asset> UpsertAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        asset = PrepareForSave(asset);

        await _mongoOperations.SaveAsync(asset, cancellationToken);
        Asset savedAsset = await _mongoOperations.GetByIdAsync(asset.Id, cancellationToken);
        PublishMaintenanceDueEventsBestEffort(savedAsset);
        return savedAsset;
    }

    private static void PublishMaintenanceDueEventsBestEffort(Asset asset)
    {
        if (asset?.MaintenanceSchedules == null)
            return;

        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            foreach (AssetMaintenanceSchedule schedule in asset.MaintenanceSchedules)
            {
                if (schedule?.NextDueOn == null || schedule.NextDueOn.Value > now)
                    continue;

                NatsInterprocess.Push(new AssetMaintenanceDueMessage()
                {
                    AssetId = asset.Id,
                    ScheduleId = schedule.Id,
                    Kind = schedule.Kind,
                    NextDueOn = schedule.NextDueOn
                });
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Unable to publish maintenance due events for {asset.Id}: {exception.Message}");
        }
    }

    public Task DeleteAsync(string assetId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(assetId);
        return normalizedId == null ? Task.CompletedTask : _mongoOperations.DeleteAsync(normalizedId, cancellationToken);
    }
}
