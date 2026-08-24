using DocFlow.ApprovalService.Application.Contracts;
using DocFlow.ApprovalService.Application.Services;
using DocFlow.BuildingBlocks.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocFlow.ApprovalService.Controllers;

[ApiController]
[Route("api/approvals")]
[Authorize]
public sealed class ApprovalsController(IApprovalService approvalService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApprovalRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var approval = await approvalService.CreateAsync(User.GetTenantId(), User.GetUserId(), request, cancellationToken);
            return CreatedAtAction(nameof(GetPending), new { id = approval.Id }, approval);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
    {
        try
        {
            var items = await approvalService.GetPendingAsync(User.GetTenantId(), User.GetUserId(), cancellationToken);
            return Ok(items);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpPost("{id:guid}/decision")]
    public async Task<IActionResult> Decide(Guid id, [FromBody] DecisionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var approval = await approvalService.DecideAsync(id, User.GetTenantId(), User.GetUserId(), request, cancellationToken);
            return approval is null ? NotFound() : Ok(approval);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }
}
