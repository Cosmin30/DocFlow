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

        var doc1 = Document.Create(
            DocFlowSeedData.TenantId,
            DocFlowSeedData.ManagerUserId,
            "Contract de servicii 2026",
            "Contracte",
            "Juridic",
            "contract,servicii,juridic",
            ConfidentialityLevel.Confidential,
            now.AddMonths(6),
            "contract-servicii-2026-v1.pdf",
            "/seed/documents/contract-servicii-2026-v1.pdf",
            245120);

        var doc2 = Document.Create(
            DocFlowSeedData.TenantId,
            DocFlowSeedData.EmployeeUserId,
            "Procedură acces intern",
            "Politici",
            "IT",
            "politica,acces,it",
            ConfidentialityLevel.Internal,
            null,
            "procedura-acces-intern-v1.pdf",
            "/seed/documents/procedura-acces-intern-v1.pdf",
            178944);

        doc2.AddVersion(
            "procedura-acces-intern-v2.pdf",
            "/seed/documents/procedura-acces-intern-v2.pdf",
            181502,
            DocFlowSeedData.EmployeeUserId);

        var doc3 = Document.Create(
            DocFlowSeedData.TenantId,
            DocFlowSeedData.AdminUserId,
            "Factură 5528",
            "Financiar",
            "Contabilitate",
            "factura,financiar,plati",
            ConfidentialityLevel.Strict,
            null,
            "factura-5528.pdf",
            "/seed/documents/factura-5528.pdf",
            96400);

        dbContext.Documents.AddRange(doc1, doc2, doc3);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
