using Microsoft.EntityFrameworkCore;
using API.Data;
using Models;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace Testing;

/* 
 * TEORI: END-TO-END INTEGRATION TESTING & DATA SEEDING
 * 
 * 1. Realistisk Systemvalidering: I modsætning til Unit Tests arbejder denne test 
 *    mod en rigtig PostgreSQL-database. Det bekræfter, at database-schema, 
 *    constraints og relationer fungerer korrekt i praksis.
 * 
 * 2. Database Isolation (IAsyncLifetime): Ved at bruge TRUNCATE og RESTART IDENTITY 
 *    sikres det, at databasen nulstilles før hver test. Dette garanterer 
 *    Idempotens – at testen altid starter fra en ren tilstand og giver samme resultat.
 * 
 * 3. Stress Testing & Time-Series Data: Ved at generere 8000 målepunkter med 
 *    realistiske tidsintervaller, stresstestes systemets evne til at håndtere 
 *    større datamængder. Dette sikrer, at historik-forespørgsler i API'et 
 *    har et solidt og realistisk fundament at arbejde med.
 */

public class EndToEndIntegrationTests : IAsyncLifetime
{
    private readonly DbContextOptions<AppDbContext> _options;

    // Konfiguration indlæses for at oprette forbindelse til test-databasen
    public EndToEndIntegrationTests()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(config.GetConnectionString("DefaultConnection"))
            .Options;
    }

    // Nulstilling af databasen før hver testkørsel for at sikre rene testdata
    public async Task InitializeAsync()
    {
        using var context = new AppDbContext(_options);
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Users\" RESTART IDENTITY CASCADE;");
    }

    public async Task DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task Mass_Seed_2_Users_8_Sets_8000_Measurements_Success()
    {
        using var context = new AppDbContext(_options);
        var random = new Random();
        
        string[] solarLocations = { "Tag", "Carport", "Terrasse", "Have" };

        // Simulation af brugeroprettelse og data-generering i stor skala
        for (int u = 1; u <= 2; u++)
        {
            // Oprettelse af testbruger med statisk PasswordHash for at undgå hashing-overhead i testen
            var newUser = new User 
            { 
                Username = $"LumenUser_{u}",
                IsAdmin = (u == 1),
                PasswordHash = "Seeding_No_Bcrypt_Used" 
            };
            
            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            // Tilknytning af en hardware-enhed til brugeren
            var newDevice = new Device 
            { 
                Id = u, 
                DeviceKey = (u == 1) ? "MKR-94b5" : "MKR-ANDEN", 
                UserId = newUser.Id,
                IsActive = false 
            };
            context.Devices.Add(newDevice);
            await context.SaveChangesAsync();

            // Generering af flere målesæt pr. enhed for at simulere historik
            for (int s = 0; s < 4; s++)
            {
                var setStartTime = DateTime.UtcNow.Date.AddDays(-s).AddHours(10);

                var newSet = new MeasurementSet
                {
                    Location = solarLocations[s],
                    StartTime = setStartTime,
                    DeviceId = newDevice.Id
                };
                context.MeasurementSets.Add(newSet);
                
                // Enheden markeres som aktiv under den simulerede måling
                newDevice.IsActive = true;
                await context.SaveChangesAsync();

                // Batch-generering af 1000 målepunkter pr. sæt baseret på en sinus-kurve
                var measurements = new List<Measurement>();
                for (int i = 0; i < 1000; i++)
                {
                    double progress = (double)i / 1000;
                    int baseValue = (int)(300 + (Math.Sin(progress * Math.PI) * 700));
                    
                    measurements.Add(new Measurement
                    {
                        MeasurementSetId = newSet.Id,
                        Value = Math.Clamp(baseValue + random.Next(-20, 20), 0, 1024),
                        Timestamp = setStartTime.AddSeconds(i * 15) 
                    });
                }
                context.Measurements.AddRange(measurements);
                
                newSet.EndTime = setStartTime.AddSeconds(1000 * 15); 
                newDevice.IsActive = false; 
                await context.SaveChangesAsync();
            }
        }
        
        // ASSERT: Verificering af at alle 8000 målinger er gemt korrekt i PostgreSQL
        var total = await context.Measurements.CountAsync();
        Assert.Equal(8000, total);
        
        // Tjek af at data-integriteten for dummy-adgangskoden er bevaret
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == "LumenUser_1");
        Assert.Equal("Seeding_No_Bcrypt_Used", user?.PasswordHash);
    }
}