namespace DocFlow.ApprovalService.Application.Contracts;

using System.ComponentModel.DataAnnotations;

public sealed record CreateApprovalRequest(
    Guid DocumentId,
    Guid AssignedToUserId,
    [StringLength(1000)] string? Comment);

public sealed record DecisionRequest(
    bool Approve,
    [StringLength(1000)] string? Comment);
