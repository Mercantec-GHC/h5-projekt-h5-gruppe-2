using System;
using Microsoft.EntityFrameworkCore;
using h5_2o_MAIN.Models;

namespace h5_2o_MAIN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Measurement> Measurements => Set<Measurement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entity
        modelBuilder.Entity<Measurement>(entity =>
        {
            // Map to specific table name (PostgreSQL convention: lowercase)
            entity.ToTable("measurements");

            // Configure primary key
            entity.HasKey(p => p.id);
        });
    }
}
