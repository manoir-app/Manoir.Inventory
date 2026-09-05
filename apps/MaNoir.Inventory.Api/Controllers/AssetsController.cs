using MaNoir.Inventory.Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory.Api.Controllers;

[ApiController]
[Route("v1.0/inventory/assets")]
public sealed class AssetsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Asset>>> GetAll(CancellationToken cancellationToken = default)
    {
        return Ok(await new AssetLogic().GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Asset>> GetById(string id, CancellationToken cancellationToken = default)
    {
        Asset asset = await new AssetLogic().GetByIdAsync(id, cancellationToken);
        return asset == null ? NotFound() : Ok(asset);
    }

    [HttpGet("byCategory/{categoryId}")]
    public async Task<ActionResult<List<Asset>>> GetByCategory(string categoryId, CancellationToken cancellationToken = default)
    {
        return Ok(await new AssetLogic().GetByCategoryAsync(categoryId, cancellationToken));
    }

    [HttpGet("maintenanceDue")]
    public async Task<ActionResult<List<Asset>>> GetWithMaintenanceDue([FromQuery] int withinDays = 0, CancellationToken cancellationToken = default)
    {
        return Ok(await new AssetLogic().GetWithMaintenanceDueAsync(withinDays, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<Asset>> Upsert([FromBody] Asset asset, CancellationToken cancellationToken = default)
    {
        return Ok(await new AssetLogic().UpsertAsync(asset, cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(string id, CancellationToken cancellationToken = default)
    {
        await new AssetLogic().DeleteAsync(id, cancellationToken);
        return Ok(true);
    }
}
