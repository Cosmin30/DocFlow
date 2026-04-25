using DocFlow.BuildingBlocks.Security;
using DocFlow.DocumentService.Application.Contracts;
using DocFlow.DocumentService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocFlow.DocumentService.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public sealed class DocumentsController(IDocumentService documentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var documents = await documentService.GetAsync(User.GetTenantId(), cancellationToken);
        return Ok(documents);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentRequest request, CancellationToken cancellationToken)
    {
        var document = await documentService.CreateAsync(User.GetTenantId(), User.GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetVersions), new { id = document.Id }, document);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDocumentRequest request, CancellationToken cancellationToken)
    {
        var document = await documentService.UpdateAsync(id, User.GetTenantId(), User.GetUserId(), request, cancellationToken);
        return document is null ? NotFound() : Ok(document);
    }

    [HttpGet("{id:guid}/versions")]
    public async Task<IActionResult> GetVersions(Guid id, CancellationToken cancellationToken)
    {
        var versions = await documentService.GetVersionsAsync(id, User.GetTenantId(), cancellationToken);
        return Ok(versions);
    }

    [HttpPost("{id:guid}/versions/{versionNumber:int}/restore")]
    public async Task<IActionResult> Restore(Guid id, int versionNumber, CancellationToken cancellationToken)
    {
        var restored = await documentService.RestoreVersionAsync(id, versionNumber, User.GetTenantId(), cancellationToken);
        return restored ? Ok() : NotFound();
    }
}
