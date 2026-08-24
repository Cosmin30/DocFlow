using DocFlow.BuildingBlocks.Security;
using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Application.CQRS.Documents.Commands.CreateDocument;
using DocFlow.DocumentService.Application.CQRS.Documents.Commands.RestoreDocumentVersion;
using DocFlow.DocumentService.Application.CQRS.Documents.Commands.UpdateDocument;
using DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocumentVersions;
using DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocuments;
using DocFlow.DocumentService.Application.Contracts;
using DocFlow.DocumentService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocFlow.DocumentService.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public sealed class DocumentsController(
    IQueryHandler<GetDocumentsQuery, List<Document>> getDocumentsHandler,
    ICommandHandler<CreateDocumentCommand, Document> createDocumentHandler,
    ICommandHandler<UpdateDocumentCommand, Document?> updateDocumentHandler,
    IQueryHandler<GetDocumentVersionsQuery, List<DocumentVersion>> getDocumentVersionsHandler,
    ICommandHandler<RestoreDocumentVersionCommand, bool> restoreDocumentVersionHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var documents = await getDocumentsHandler.Handle(new GetDocumentsQuery(User.GetTenantId()), cancellationToken);
            return Ok(documents);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var document = await createDocumentHandler.Handle(
                new CreateDocumentCommand(User.GetTenantId(), User.GetUserId(), request),
                cancellationToken);

            return CreatedAtAction(nameof(GetVersions), new { id = document.Id }, document);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDocumentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var document = await updateDocumentHandler.Handle(
                new UpdateDocumentCommand(id, User.GetTenantId(), User.GetUserId(), request),
                cancellationToken);

            return document is null ? NotFound() : Ok(document);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpGet("{id:guid}/versions")]
    public async Task<IActionResult> GetVersions(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var versions = await getDocumentVersionsHandler.Handle(
                new GetDocumentVersionsQuery(id, User.GetTenantId()),
                cancellationToken);

            return Ok(versions);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpPost("{id:guid}/versions/{versionNumber:int}/restore")]
    public async Task<IActionResult> Restore(Guid id, int versionNumber, CancellationToken cancellationToken)
    {
        try
        {
            var restored = await restoreDocumentVersionHandler.Handle(
                new RestoreDocumentVersionCommand(id, versionNumber, User.GetTenantId(), User.GetUserId()),
                cancellationToken);

            return restored ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }
}
