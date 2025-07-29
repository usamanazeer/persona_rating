using AuthService.Domain.Constants;
using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedRolesAsync(AuthDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return; // Roles already seeded

        var roles = new List<Role>
        {
            new Role
            {
                Id = RoleConstants.Ids.RegisteredUser,
                Name = RoleConstants.Names.RegisteredUser,
                Description = "Create profiles, write reviews, subscribe",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = RoleConstants.Ids.Verifier,
                Name =  RoleConstants.Names.Verifier,
                Description = "Validate profile submissions",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = RoleConstants.Ids.AdminBackOffice,
                Name = RoleConstants.Names.AdminBackOffice,
                Description = "Moderate content, finalize verifications",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = RoleConstants.Ids.PublicFigure,
                Name = RoleConstants.Names.PublicFigure,
                Description = "Officially engage with their own claimed profile",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    public static async Task SeedAsync(AuthDbContext context)
    {
        await SeedRolesAsync(context);
    }
} 