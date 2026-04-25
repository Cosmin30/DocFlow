namespace DocFlow.ApprovalService.Application.Contracts;

using System.ComponentModel.DataAnnotations;

public sealed record CreateApprovalRequest(
    Guid DocumentId,
    Guid AssignedToUserId,
    [property: StringLength(1000)] string? Comment);

public sealed record DecisionRequest(
    bool Approve,
    [property: StringLength(1000)] string? Comment);
