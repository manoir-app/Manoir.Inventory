using MaNoir.Inventory.Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory.Api.Controllers;

[ApiController]
[Route("v1.0/inventory/categories")]
public sealed class CategoriesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Category>>> GetAll(CancellationToken cancellationToken = default)
    {
        return Ok(await new CategoryLogic().GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(string id, CancellationToken cancellationToken = default)
    {
        Category category = await new CategoryLogic().GetByIdAsync(id, cancellationToken);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Upsert([FromBody] Category category, CancellationToken cancellationToken = default)
    {
        return Ok(await new CategoryLogic().UpsertAsync(category, cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(string id, CancellationToken cancellationToken = default)
    {
        bool deleted = await new CategoryLogic().DeleteAsync(id, cancellationToken);
        return deleted ? Ok(true) : Conflict(false);
    }
}
