using DocFlow.AuditService.Domain.Entities;
using DocFlow.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.AuditService.Infrastructure.Persistence;

public static class AuditSeedExtensions
{
    public static async Task SeedAsync(this AuditDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.AuditLogs.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;

        await dbContext.AuditLogs.AddRangeAsync(new[]
        {
            new AuditLog
            {
                TenantId = DocFlowSeedData.TenantId,
                UserId = DocFlowSeedData.ManagerUserId,
                Action = "Document uploaded",
                EntityType = "Document",
                EntityId = DocFlowSeedData.FirstDocumentId.ToString(),
                MetadataJson = "{\"title\":\"Contract de servicii 2026\"}",
                IpAddress = "127.0.0.1",
                Device = "Seeder",
                CreatedAtUtc = now.AddDays(-12)
            },
            new AuditLog
            {
                TenantId = DocFlowSeedData.TenantId,
                UserId = DocFlowSeedData.EmployeeUserId,
                Action = "Approval requested",
                EntityType = "ApprovalRequest",
                EntityId = DocFlowSeedData.FirstApprovalId.ToString(),
                MetadataJson = "{\"status\":\"Pending\"}",
                IpAddress = "127.0.0.1",
                Device = "Seeder",
                CreatedAtUtc = now.AddDays(-1)
            },
            new AuditLog
            {
                TenantId = DocFlowSeedData.TenantId,
                UserId = DocFlowSeedData.AdminUserId,
                Action = "Document approved",
                EntityType = "ApprovalRequest",
                EntityId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa").ToString(),
                MetadataJson = "{\"status\":\"Approved\"}",
                IpAddress = "127.0.0.1",
                Device = "Seeder",
                CreatedAtUtc = now.AddDays(-3)
            },
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}