using System.Diagnostics;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

/* 
 * RESULTAT FRA TESTKØRSEL:
 * - ~13.000 målinger/sekund.
 * - Total tid for 100.000 rækker: ~7,6 sekunder.
 * 
 * KONKLUSION:
 *    Skalerbarhed: Ved et 15-sekunders måleinterval kan databasen teoretisk 
 *    håndtere over 195.000 enheder (13.000 x 15), før skrivning til disk 
 *    bliver en flaskehals. Testen bekræfter derfor, at Docker-containeren og 
 *    serverens Disk I/O på den isolerede port 5434 leverer den nødvendige 
 *    performance til et professionelt produktionsmiljø.
 *
 * KOMMANDOER TIL KØRSEL AF TESTS:
 * 
 * 1. For at køre alle normale hurtige tests (eksklusiv stresstest):
 *    dotnet test --filter "Category!=Performance"
 * 
 * 2. For at køre KUN stresstesten (med detaljeret output):
 *    dotnet test --filter "Category=Performance" --logger "console;verbosity=detailed"
 */

namespace Testing;

/* 
 * TEORI: PERFORMANCE TESTING & HORIZONTAL SCALING
 * 
 * 1. Performance Benchmarking: Denne test måler systemets maksimale kapacitet ved at 
 *    omgå API- og ORM-lagene. Ved at bruge Raw SQL og NpgsqlBatch testes den rå 
 *    gennemstrømning direkte mod PostgreSQL, hvilket identificerer hardwarens øvre grænse.
 * 
 * 2. Resource Isolation: Ved at benytte en dedikeret port (5433) isoleres stresstesten 
 *    fra den primære database. Dette sikrer, at belastningen ikke påvirker systemets 
 *    øvrige funktioner eller forurener produktionsdata.
 * 
 * 3. Throughput Analysis: Beregningen af operationer pr. sekund (TPS) giver et 
 *    faktuelt grundlag for at vurdere systemets skalerbarhed. Det gør det muligt 
 *    at forudsige, hvornår der opstår behov for arkitektoniske ændringer som 
 *    database-sharding eller hardware-opgradering.
 */

public class RawDatabaseStressTests
{
    private readonly string _connectionString;
    private readonly ITestOutputHelper _output;

    // Konfiguration indlæses for at oprette forbindelse til den isolerede stresstest-database
    public RawDatabaseStressTests(ITestOutputHelper output)
    {
        _output = output;
        
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        _connectionString = config.GetConnectionString("StressConnection") 
            ?? throw new Exception("StressConnection mangler i appsettings.json");
    }

    [Fact]
    [Trait("Category", "Performance")] // Muliggør filtrering så stresstesten ikke kører med unit tests
    public async Task StressTest_RawSql_Insert_Performance()
    {
        // ARRANGE: Definition af test-volumen på 100.000 rækker
        int totalRows = 100000;
        int dummySetId = 999; 
        var timer = new Stopwatch();

        _output.WriteLine("Starter isoleret stresstest (Raw SQL)...");

        // ACT: Udførelse af bulk-indsættelse via NpgsqlBatch for maksimal performance
        await using var dataSource = NpgsqlDataSource.Create(_connectionString);
        timer.Start();

        await using (var conn = await dataSource.OpenConnectionAsync())
        await using (var batch = new NpgsqlBatch(conn))
        {
            for (int i = 0; i < totalRows; i++)
            {
                // Raw SQL benyttes for at fjerne overhead fra Entity Framework
                var cmd = new NpgsqlBatchCommand(@"INSERT INTO ""Measurements"" (""Value"", ""Timestamp"", ""MeasurementSetId"") 
                                                  VALUES ($1, $2, $3)");
                
                cmd.Parameters.AddWithValue(new Random().Next(0, 1024));
                cmd.Parameters.AddWithValue(DateTime.UtcNow);
                cmd.Parameters.AddWithValue(dummySetId);
                
                batch.BatchCommands.Add(cmd);
            }
            // Hele batchen sendes som én samlet netværkspakke til PostgreSQL
            await batch.ExecuteNonQueryAsync();
        }

        timer.Stop();

        // ASSERT & RESULTS: Beregning og validering af gennemstrømning
        var opsPerSecond = totalRows / timer.Elapsed.TotalSeconds;
        _output.WriteLine("--------------------------------------------------");
        _output.WriteLine($"STRESSTEST RESULTAT:");
        _output.WriteLine($"Hastighed: {opsPerSecond:F0} rækker/sekund");
        _output.WriteLine($"Total tid: {timer.Elapsed.TotalSeconds:F2} sekunder");
        _output.WriteLine("--------------------------------------------------");

        // Testen fejler hvis databasen ikke kan håndtere mindst 1000 målinger pr. sekund
        Assert.True(opsPerSecond > 1000, "Database performance er under det acceptable niveau for bulk-ingestion.");
    }
}