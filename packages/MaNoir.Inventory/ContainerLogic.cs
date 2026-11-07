using MaNoir.Inventory.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory;

public sealed class ContainerLogic
{
    private readonly ContainerMongoOperations _mongoOperations = new();

    public Task<Container> GetByIdAsync(string containerId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(containerId);
        return normalizedId == null ? Task.FromResult<Container>(null) : _mongoOperations.GetByIdAsync(normalizedId, cancellationToken);
    }

    public Task<List<Container>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _mongoOperations.GetAllAsync(cancellationToken);
    }

    public static Container PrepareForSave(Container container)
    {
        ArgumentNullException.ThrowIfNull(container);
        if (string.IsNullOrWhiteSpace(container.Label))
            throw new ArgumentException("The container label cannot be empty.", nameof(container));

        bool isNew = string.IsNullOrWhiteSpace(container.Id);
        container.Id = isNew ? InventoryIdHelper.NewId() : InventoryIdHelper.NormalizeId(container.Id);
        container.Audit ??= new AuditInfo();
        container.Audit.CreatedOn = isNew ? DateTimeOffset.UtcNow : container.Audit.CreatedOn;
        container.Audit.UpdatedOn = isNew ? null : DateTimeOffset.UtcNow;

        foreach (ContainerSubElement subElement in container.SubElements ?? [])
        {
            if (string.IsNullOrWhiteSpace(subElement.Id))
                subElement.Id = InventoryIdHelper.NewId();
        }

        return container;
    }

    public async Task<Container> UpsertAsync(Container container, CancellationToken cancellationToken = default)
    {
        container = PrepareForSave(container);

        await _mongoOperations.SaveAsync(container, cancellationToken);
        return await _mongoOperations.GetByIdAsync(container.Id, cancellationToken);
    }

    public Task DeleteAsync(string containerId, CancellationToken cancellationToken = default)
    {
        string normalizedId = InventoryIdHelper.NormalizeId(containerId);
        return normalizedId == null ? Task.CompletedTask : _mongoOperations.DeleteAsync(normalizedId, cancellationToken);
    }
}
