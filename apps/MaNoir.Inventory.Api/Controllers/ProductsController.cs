using MaNoir.Inventory.Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory.Api.Controllers;

[ApiController]
[Route("v1.0/inventory/products")]
public sealed class ProductsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll(CancellationToken cancellationToken = default)
    {
        return Ok(await new ProductLogic().GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(string id, CancellationToken cancellationToken = default)
    {
        Product product = await new ProductLogic().GetByIdAsync(id, cancellationToken);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpGet("byEan/{ean}")]
    public async Task<ActionResult<Product>> GetByEan(string ean, CancellationToken cancellationToken = default)
    {
        Product product = await new ProductLogic().GetByEanAsync(ean, cancellationToken);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpGet("byCategory/{categoryId}")]
    public async Task<ActionResult<List<Product>>> GetByCategory(string categoryId, CancellationToken cancellationToken = default)
    {
        return Ok(await new ProductLogic().GetByCategoryAsync(categoryId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Upsert([FromBody] Product product, CancellationToken cancellationToken = default)
    {
        return Ok(await new ProductLogic().UpsertAsync(product, cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(string id, CancellationToken cancellationToken = default)
    {
        await new ProductLogic().DeleteAsync(id, cancellationToken);
        return Ok(true);
    }
}
