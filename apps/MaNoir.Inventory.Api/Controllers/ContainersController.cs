using MaNoir.Inventory.Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaNoir.Inventory.Api.Controllers;

[ApiController]
[Route("v1.0/inventory/containers")]
public sealed class ContainersController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Container>>> GetAll(CancellationToken cancellationToken = default)
    {
        return Ok(await new ContainerLogic().GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Container>> GetById(string id, CancellationToken cancellationToken = default)
    {
        Container container = await new ContainerLogic().GetByIdAsync(id, cancellationToken);
        return container == null ? NotFound() : Ok(container);
    }

    [HttpPost]
    public async Task<ActionResult<Container>> Upsert([FromBody] Container container, CancellationToken cancellationToken = default)
    {
        return Ok(await new ContainerLogic().UpsertAsync(container, cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(string id, CancellationToken cancellationToken = default)
    {
        await new ContainerLogic().DeleteAsync(id, cancellationToken);
        return Ok(true);
    }
}
