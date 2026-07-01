using DocFlow.AuthService.Domain.Entities;
using DocFlow.AuthService.Infrastructure.Security;
using DocFlow.BuildingBlocks;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.AuthService.Infrastructure.Persistence;

public static class AuthSeedExtensions
{
    public static async Task SeedAsync(this AuthDbContext dbContext, IPasswordHasher passwordHasher, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Tenants.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;
        var passwordHash = passwordHasher.Hash(DocFlowSeedData.DefaultPassword);

        await dbContext.Tenants.AddAsync(new Tenant
        {
            Id = DocFlowSeedData.TenantId,
            Name = DocFlowSeedData.TenantName,
            Slug = DocFlowSeedData.TenantSlug,
            CreatedAtUtc = now
        }, cancellationToken);

        await dbContext.Users.AddRangeAsync(new[]
        {
            new User
            {
                Id = DocFlowSeedData.AdminUserId,
                TenantId = DocFlowSeedData.TenantId,
                Email = DocFlowSeedData.AdminEmail,
                FullName = "Admin DocFlow",
                PasswordHash = passwordHash,
                Role = UserRole.CompanyAdmin,
                CreatedAtUtc = now
            },
            new User
            {
                Id = DocFlowSeedData.ManagerUserId,
                TenantId = DocFlowSeedData.TenantId,
                Email = DocFlowSeedData.ManagerEmail,
                FullName = "Manager DocFlow",
                PasswordHash = passwordHash,
                Role = UserRole.Manager,
                CreatedAtUtc = now
            },
            new User
            {
                Id = DocFlowSeedData.AuditorUserId,
                TenantId = DocFlowSeedData.TenantId,
                Email = DocFlowSeedData.AuditorEmail,
                FullName = "Auditor DocFlow",
                PasswordHash = passwordHash,
                Role = UserRole.ReadOnlyAuditor,
                CreatedAtUtc = now
            },
            new User
            {
                Id = DocFlowSeedData.EmployeeUserId,
                TenantId = DocFlowSeedData.TenantId,
                Email = DocFlowSeedData.EmployeeEmail,
                FullName = "Angajat DocFlow",
                PasswordHash = passwordHash,
                Role = UserRole.Employee,
                CreatedAtUtc = now
            },
            }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}