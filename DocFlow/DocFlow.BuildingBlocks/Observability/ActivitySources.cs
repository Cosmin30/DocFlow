using System.Diagnostics;

namespace DocFlow.BuildingBlocks.Observability;

public static class ActivitySources
{
    public const string ServiceName = "DocFlow";
    public static readonly ActivitySource DocumentService = new($"{ServiceName}.DocumentService");
    public static readonly ActivitySource AuthService = new($"{ServiceName}.AuthService");
    public static readonly ActivitySource ApprovalService = new($"{ServiceName}.ApprovalService");
    public static readonly ActivitySource AuditService = new($"{ServiceName}.AuditService");
    public static readonly ActivitySource Gateway = new($"{ServiceName}.Gateway");
}
