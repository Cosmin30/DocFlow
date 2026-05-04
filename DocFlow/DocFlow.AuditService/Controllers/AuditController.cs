using DocFlow.AuditService.Application.Contracts;
using DocFlow.AuditService.Application.Services;
using DocFlow.BuildingBlocks.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocFlow.AuditService.Controllers;

[ApiController]
[Route("api/audit")]
[Authorize]
public sealed class AuditController(IAuditService auditService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Write([FromBody] WriteAuditRequest request, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var device = HttpContext.Request.Headers.UserAgent.ToString();
        var audit = await auditService.WriteAsync(User.GetTenantId(), User.GetUserId(), ip, device, request, cancellationToken);
        return CreatedAtAction(nameof(List), new { id = audit.Id }, audit);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int take = 100, CancellationToken cancellationToken = default)
    {
        var logs = await auditService.ListAsync(User.GetTenantId(), take, cancellationToken);
        return Ok(logs);
    }
}
