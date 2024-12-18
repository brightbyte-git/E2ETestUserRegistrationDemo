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

        // Primary Key
        modelBuilder.Entity<Tenant>()
            .HasKey(t => t.Id);

        // Unique Constraint for Name
        modelBuilder.Entity<Tenant>()
            .HasIndex(t => t.Name)
            .IsUnique();
    }
}