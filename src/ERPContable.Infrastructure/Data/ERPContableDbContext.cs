using ERPContable.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERPContable.Infrastructure.Data;

public class ERPContableDbContext : DbContext
{
    public ERPContableDbContext(DbContextOptions<ERPContableDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.RazonSocial).IsRequired().HasMaxLength(255);
            entity.Property(t => t.RFC).IsRequired().HasMaxLength(13);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
        });
    }
}