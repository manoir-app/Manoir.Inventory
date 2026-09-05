using MaNoir.Inventory.Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory.Api.Controllers;

[ApiController]
[Route("v1.0/inventory/stock")]
public sealed class ProductStocksController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductStock>> GetById(string id, CancellationToken cancellationToken = default)
    {
        ProductStock stock = await new ProductStockLogic().GetByIdAsync(id, cancellationToken);
        return stock == null ? NotFound() : Ok(stock);
    }

    [HttpGet("byProduct/{productId}")]
    public async Task<ActionResult<List<ProductStock>>> GetByProduct(string productId, CancellationToken cancellationToken = default)
    {
        return Ok(await new ProductStockLogic().GetByProductIdAsync(productId, cancellationToken));
    }

    [HttpGet("byContainer/{containerId}")]
    public async Task<ActionResult<List<ProductStock>>> GetByContainer(string containerId, CancellationToken cancellationToken = default)
    {
        return Ok(await new ProductStockLogic().GetByContainerIdAsync(containerId, cancellationToken));
    }

    [HttpGet("low")]
    public async Task<ActionResult<List<ProductStock>>> GetLowStock(CancellationToken cancellationToken = default)
    {
        return Ok(await new ProductStockLogic().GetLowStockAsync(cancellationToken));
    }

    [HttpGet("expiring")]
    public async Task<ActionResult<List<ProductStock>>> GetExpiring([FromQuery] int withinDays = 0, CancellationToken cancellationToken = default)
    {
        return Ok(await new ProductStockLogic().GetExpiringAsync(withinDays, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ProductStock>> Upsert([FromBody] ProductStock stock, CancellationToken cancellationToken = default)
    {
        return Ok(await new ProductStockLogic().UpsertAsync(stock, cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(string id, CancellationToken cancellationToken = default)
    {
        await new ProductStockLogic().DeleteAsync(id, cancellationToken);
        return Ok(true);
    }
}
