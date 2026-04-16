using Microsoft.EntityFrameworkCore;
using API.Data; 
using Xunit;
using Microsoft.Extensions.Configuration;

namespace Testing;

/* 
 * TEORI: INFRASTRUCTURE TESTING & SMOKE TESTING
 * 
 * 1. Infrastructure Testing: Denne test verificerer fundamentet for hele applikationen. 
 *    Uden en stabil databaseforbindelse kan repositories og controllers ikke fungere. 
 *    Testen sikrer, at netværkskonfiguration, firewalls og adgangskoder er korrekte.
 * 
 * 2. Smoke Testing: Testen fungerer som en "røg-test" (smoke test). Det er en hurtig, 
 *    overordnet test, der bekræfter, at de mest kritiske dele af systemet er 
 *    operationelle, før mere komplekse integrationstests køres.
 * 
 * 3. Configuration Validation: Ved at indlæse appsettings.json valideres det, at 
 *    miljøvariabler og konfigurationsfiler er korrekt opsat i testmiljøet, hvilket 
 *    minimerer risikoen for fejl, der skyldes manglende opsætning fremfor kodefejl.
 */

public class DatabaseConnectionTests
{
    private readonly string _connectionString;

    public DatabaseConnectionTests()
    {
        // Indlæsning af systemets konfiguration for at finde forbindelsesstrenge
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        _connectionString = config.GetConnectionString("DefaultConnection") 
            ?? throw new Exception("ConnectionString 'DefaultConnection' ikke fundet!");
    }

    [Fact]
    public async Task Database_CanConnect_Successfully()
    {
        // ARRANGE: Opsætning af database-konteksten med den fundne forbindelsesstreng
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        using var context = new AppDbContext(options);

        // ACT: Udførelse af et asynkront forbindelsestjek mod PostgreSQL-serveren
        // Dette tjekker både netværksadgang og brugerrettigheder
        bool canConnect = await context.Database.CanConnectAsync();

        // ASSERT: Verificering af at forbindelsen rent faktisk blev etableret
        Assert.True(canConnect, "Forbindelse til PostgreSQL-databasen fejlede.");
    }
}