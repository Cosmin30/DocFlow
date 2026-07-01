using DocFlow.ApprovalService.Domain.Entities;
using DocFlow.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.ApprovalService.Infrastructure.Persistence;

public static class ApprovalSeedExtensions
{
    public static async Task SeedAsync(this ApprovalDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Approvals.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;

        await dbContext.Approvals.AddRangeAsync(new[]
        {
            new ApprovalRequest
            {
                Id = DocFlowSeedData.FirstApprovalId,
                TenantId = DocFlowSeedData.TenantId,
                DocumentId = DocFlowSeedData.FirstDocumentId,
                RequestedByUserId = DocFlowSeedData.ManagerUserId,
                AssignedToUserId = DocFlowSeedData.AdminUserId,
                Status = ApprovalStatus.Pending,
                Comment = "Necesită verificare juridică.",
                CreatedAtUtc = now.AddDays(-1)
            },
            new ApprovalRequest
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                TenantId = DocFlowSeedData.TenantId,
                DocumentId = DocFlowSeedData.SecondDocumentId,
                RequestedByUserId = DocFlowSeedData.EmployeeUserId,
                AssignedToUserId = DocFlowSeedData.ManagerUserId,
                Status = ApprovalStatus.Approved,
                Comment = "Aprobat pentru folosire internă.",
                CreatedAtUtc = now.AddDays(-4),
                ResolvedAtUtc = now.AddDays(-3)
            },
            new ApprovalRequest
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                TenantId = DocFlowSeedData.TenantId,
                DocumentId = DocFlowSeedData.ThirdDocumentId,
                RequestedByUserId = DocFlowSeedData.AdminUserId,
                AssignedToUserId = DocFlowSeedData.AuditorUserId,
                Status = ApprovalStatus.Rejected,
                Comment = "Lipsesc încă date de verificare.",
                CreatedAtUtc = now.AddDays(-2),
                ResolvedAtUtc = now.AddDays(-1)
            },
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}