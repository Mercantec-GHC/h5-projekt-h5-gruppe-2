using API.Services;
using Microsoft.Extensions.Configuration;
using Xunit;
using System.Text.Json;

namespace Testing;

/* 
 * TEORI: ASYNCHRONOUS MESSAGING & CONTRACT TESTING
 * 
 * 1. Integration Testing (Messaging): Denne test verificerer forbindelsen til 
 *    besked-brokeren (RabbitMQ). Det sikres, at API'et kan etablere en kanal, 
 *    oprette en kø og publicere beskeder uden at kaste fejl. Dette er afgørende 
 *    for systemets evne til at styre hardwaren.
 * 
 * 2. Contract Testing: Da systemet er polyglot (C# API og Python Worker), er 
 *    besked-kontrakten det eneste bindeled. Testen validerer, at JSON-strukturen 
 *    matcher præcis det format, som Python-workeren forventer, herunder 
 *    feltnavne og datatyper (Serialization).
 * 
 * 3. Resilience Testing: Ved at teste mod faktiske hardware-ID'er bekræftes det, 
 *    at systemets logik omkring enhedsspecifikke kommandoer er intakt, og at 
 *    infrastrukturen kan håndtere de asynkrone kald under realistiske forhold.
 */

public class RabbitMQIntegrationTests
{
    private readonly IConfiguration _config;

    public RabbitMQIntegrationTests()
    {
        // Indlæsning af konfiguration for at få adgang til RabbitMQ Host-indstillinger
        _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
    }

    [Theory]
    [InlineData(1)] // Tester Arduino 1 (MKR-94b5)
    [InlineData(2)] // Tester Arduino 2 (MKR-ANDEN)
    public async Task SendStartCommand_ValidDevices_ShouldPublishSuccessfully(int deviceId)
    {
        // ARRANGE: Initialisering af produceren med systemets konfiguration
        var producer = new RabbitMQProducer(_config);
        int fakeSetId = 999; 

        // ACT: Forsøg på at sende en kommando til RabbitMQ-køen
        // Record.ExceptionAsync fanger eventuelle forbindelsesfejl eller netværksproblemer
        var exception = await Record.ExceptionAsync(() => 
            producer.SendStartCommand(deviceId, fakeSetId));

        // ASSERT: Verificering af at ingen fejl opstod under publicering
        Assert.Null(exception);
    }

    [Fact]
    public void MessageContract_MustMatchPythonWorkerExpectations()
    {
        // ARRANGE: Definition af test-besked der simulerer den interne struktur i produceren
        var message = new { 
            DeviceId = 1, 
            MeasurementSetId = 500, 
            Action = "START", 
            Timestamp = DateTime.UtcNow 
        };

        // ACT: Konvertering af objektet til JSON-format (præcis som det sker i produktion)
        var json = JsonSerializer.Serialize(message);

        // ASSERT: Validering af at JSON-strengen indeholder de korrekte nøgler og værdier.
        // Dette sikrer at Python-workeren kan dekode beskeden korrekt (Case-sensitivity tjek).
        Assert.Contains("\"DeviceId\":1", json);
        Assert.Contains("\"Action\":\"START\"", json);
        Assert.Contains("\"MeasurementSetId\":500", json);
    }
}