using DocFlow.BuildingBlocks.Security;
using DocFlow.DocumentService.Application.CQRS.Documents.Commands.CreateDocument;
using DocFlow.DocumentService.Application.CQRS.Documents.Commands.RestoreDocumentVersion;
using DocFlow.DocumentService.Application.CQRS.Documents.Commands.UpdateDocument;
using DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocumentVersions;
using DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocuments;
using DocFlow.DocumentService.Application.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocFlow.DocumentService.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public sealed class DocumentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var documents = await mediator.Send(new GetDocumentsQuery(User.GetTenantId()), cancellationToken);
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
            var document = await mediator.Send(
                new CreateDocumentCommand(User.GetTenantId(), User.GetUserId(), request),
                cancellationToken);

            return CreatedAtAction(nameof(GetVersions), new { id = document.Id }, document);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Errors.Select(e => new Microsoft.AspNetCore.Mvc.ModelStateModelError(e.PropertyName, e.ErrorMessage)).ToArray());
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
            var document = await mediator.Send(
                new UpdateDocumentCommand(id, User.GetTenantId(), User.GetUserId(), request),
                cancellationToken);

            return document is null ? NotFound() : Ok(document);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Errors.Select(e => new Microsoft.AspNetCore.Mvc.ModelStateModelError(e.PropertyName, e.ErrorMessage)).ToArray());
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
            var versions = await mediator.Send(
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
            var restored = await mediator.Send(
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
