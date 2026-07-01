using DocFlow.BuildingBlocks;
using DocFlow.DocumentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.DocumentService.Infrastructure.Persistence;

public static class DocumentSeedExtensions
{
    public static async Task SeedAsync(this DocumentDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Documents.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;

        await dbContext.Documents.AddRangeAsync(new[]
        {
            new Document
            {
                Id = DocFlowSeedData.FirstDocumentId,
                TenantId = DocFlowSeedData.TenantId,
                OwnerUserId = DocFlowSeedData.ManagerUserId,
                Title = "Contract de servicii 2026",
                Category = "Contracte",
                Department = "Juridic",
                TagsCsv = "contract,servicii,juridic",
                ConfidentialityLevel = ConfidentialityLevel.Confidential,
                ExpiresAtUtc = now.AddMonths(6),
                CurrentVersionNumber = 1,
                CreatedAtUtc = now.AddDays(-12)
            },
            new Document
            {
                Id = DocFlowSeedData.SecondDocumentId,
                TenantId = DocFlowSeedData.TenantId,
                OwnerUserId = DocFlowSeedData.EmployeeUserId,
                Title = "Procedură acces intern",
                Category = "Politici",
                Department = "IT",
                TagsCsv = "politica,acces,it",
                ConfidentialityLevel = ConfidentialityLevel.Internal,
                CurrentVersionNumber = 2,
                CreatedAtUtc = now.AddDays(-7)
            },
            new Document
            {
                Id = DocFlowSeedData.ThirdDocumentId,
                TenantId = DocFlowSeedData.TenantId,
                OwnerUserId = DocFlowSeedData.AdminUserId,
                Title = "Factură 5528",
                Category = "Financiar",
                Department = "Contabilitate",
                TagsCsv = "factura,financiar,plati",
                ConfidentialityLevel = ConfidentialityLevel.Strict,
                CurrentVersionNumber = 1,
                CreatedAtUtc = now.AddDays(-3)
            },
        }, cancellationToken);

        await dbContext.DocumentVersions.AddRangeAsync(new[]
        {
            new DocumentVersion
            {
                DocumentId = DocFlowSeedData.FirstDocumentId,
                VersionNumber = 1,
                FileName = "contract-servicii-2026-v1.pdf",
                StoragePath = "/seed/documents/contract-servicii-2026-v1.pdf",
                SizeBytes = 245120,
                UploadedByUserId = DocFlowSeedData.ManagerUserId,
                CreatedAtUtc = now.AddDays(-12)
            },
            new DocumentVersion
            {
                DocumentId = DocFlowSeedData.SecondDocumentId,
                VersionNumber = 1,
                FileName = "procedura-acces-intern-v1.pdf",
                StoragePath = "/seed/documents/procedura-acces-intern-v1.pdf",
                SizeBytes = 178944,
                UploadedByUserId = DocFlowSeedData.EmployeeUserId,
                CreatedAtUtc = now.AddDays(-7)
            },
            new DocumentVersion
            {
                DocumentId = DocFlowSeedData.SecondDocumentId,
                VersionNumber = 2,
                FileName = "procedura-acces-intern-v2.pdf",
                StoragePath = "/seed/documents/procedura-acces-intern-v2.pdf",
                SizeBytes = 181502,
                UploadedByUserId = DocFlowSeedData.EmployeeUserId,
                CreatedAtUtc = now.AddDays(-5)
            },
            new DocumentVersion
            {
                DocumentId = DocFlowSeedData.ThirdDocumentId,
                VersionNumber = 1,
                FileName = "factura-5528.pdf",
                StoragePath = "/seed/documents/factura-5528.pdf",
                SizeBytes = 96400,
                UploadedByUserId = DocFlowSeedData.AdminUserId,
                CreatedAtUtc = now.AddDays(-3)
            },
            }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}