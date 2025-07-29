using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            // Seed roles
            entity.HasData(
                new Role { Id = 1, Name = "Registered User", Description = "Create profiles, write reviews, subscribe", CreatedAt = DateTime.UtcNow },
                new Role { Id = 2, Name = "Verifier", Description = "Validate profile submissions", CreatedAt = DateTime.UtcNow },
                new Role { Id = 3, Name = "Admin", Description = "Moderate content, finalize verifications", CreatedAt = DateTime.UtcNow },
                new Role { Id = 4, Name = "Public Figure", Description = "Officially engage with their own claimed profile", CreatedAt = DateTime.UtcNow }
            );
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.GoogleId).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Id).HasMaxLength(450);
            entity.Property(e => e.GoogleId).HasMaxLength(450);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Picture).HasMaxLength(500);
            
            // Configure relationship with Role
            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.Id).HasMaxLength(450);
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.Token).HasMaxLength(500);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 