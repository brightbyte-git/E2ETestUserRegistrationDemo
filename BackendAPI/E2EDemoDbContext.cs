using BackendAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI;

public class E2EDemoDbContext : DbContext
{
    public E2EDemoDbContext(DbContextOptions<E2EDemoDbContext> options) : base(options) { }
    
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Tenant> Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Tenant composite primary key
        modelBuilder.Entity<Tenant>().HasKey(t => new { t.Id, t.Name });
        
        // Configure Tenant relationships
        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => new { u.TenantId, u.Organisation });
    }
}