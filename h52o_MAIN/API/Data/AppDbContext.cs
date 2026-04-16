using System;
using Microsoft.EntityFrameworkCore;
using Models;

namespace API.Data;

public class AppDbContext : DbContext
{
    // Constructor modtager indstillinger fra ConnectionString
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets repræsenterer de faktiske tabeller i databasen
    public DbSet<User> Users => Set<User>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<MeasurementSet> MeasurementSets => Set<MeasurementSet>();
    public DbSet<Measurement> Measurements => Set<Measurement>();

    // Konfiguration af tabeller og relationer
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. User Konfiguration: Sikring af unikke brugernavne og påkrævet sikkerheds-hash
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.IsAdmin).HasDefaultValue(false);

            // Definition af 1-til-1 relation mellem User og Device
            entity.HasOne(u => u.Device)
                .WithOne()
                .HasForeignKey<Device>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 2. Device Konfiguration: Unik identifikator for hardwaren og aktivitetsstatus
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasIndex(d => d.DeviceKey).IsUnique();
            entity.Property(d => d.IsActive).HasDefaultValue(false);

            // 1-til-mange relation: En enhed kan have mange målesæt (historik)
            entity.HasMany(d => d.Sets)
                .WithOne(s => s.Device)
                .HasForeignKey(s => s.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 3. MeasurementSet Konfiguration: Metadata om den specifikke måle-session
        modelBuilder.Entity<MeasurementSet>(entity =>
        {
            entity.Property(s => s.Location);
            entity.Property(s => s.StartTime).IsRequired();

            // 1-til-mange relation: Et målesæt indeholder mange individuelle målinger
            entity.HasMany(s => s.Measurements)
                .WithOne(m => m.MeasurementSet)
                .HasForeignKey(m => m.MeasurementSetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 4. Measurement Konfiguration: Præcis definition af måleværdi og tidsstempel
        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.Property(m => m.Value).IsRequired();
            entity.Property(m => m.Timestamp).IsRequired();
        });

        // 5. Global PostgreSQL DateTime håndtering:
        // Konverterer automatisk alle DateTime-egenskaber til PostgreSQL's foretrukne 
        // format (UTC med tidszone) for at sikre konsistens på tværs af server og hardware.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

            foreach (var property in properties)
            {
                property.SetColumnType("timestamp with time zone");
            }
        }
    }
}