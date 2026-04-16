
markdown
# h52o API

Dette projekt er en backend-løsning til opsamling og monitorering af solenergidata fra IoT-enheder. 
Systemet er bygget med fokus på sikkerhed, skalerbarhed og realtids-kommunikation.

## Repository og Download
For at hente projektet lokalt kan følgende kommando bruges:
```bash
git clone https://github.com
```

## Dependencies
API'en anvender følgende biblioteker (NuGet), som findes i projektfilerne.

1. Microsoft.EntityFrameworkCore (10.0.5) - Primær, gør det muligt at arbejde med databaser som C#-objekter i stedet for rå SQL.
2. Microsoft.EntityFrameworkCore.Design (10.0.5) - Ansvarlig for migrations samt opdatering databasen.
3. Microsoft.EntityFrameworkCore.Tools (10.0.5) - Muliggøre kommandoer såsom dotnet ef migrations add mv.
4. Microsoft.VisualStudio.Web.CodeGeneration.Design (10.0.2) - Hjælpeværktøj til at autogenerere f.esk. controllers gennem scafolding i VSCode.
5. Npgsql.EntityFrameworkCore.PostgreSQL (10.0.1) - Gør det muligt for EF Core at kommunikere specifikt med PostgreSQL databasen.
6. Swashbuckle.AspNetCore (10.1.7) - Bygger Swagger-side med visuel oversigt over endpoints.
7. Microsoft.AspNetCore.Authentication.JwtBearer (10.0.5) - Giver API'en evnen til at læse og validere JWT-tokens.
8. BCrypt.Net-Next (4.1.0) - Sikkerhedsbibliotek der hasher adgangskoder.
9. Microsoft.AspNetCore.OpenApi (10.0.5) - Hjælpepakke til beskrivelse af endpoints efter officiel "OpenAPI" standard, som Swagger bruger.
10. RabbitMQ.Client (7.2.1) - Gør det muligt for API'en at sende og modtage beskeder via RabbitMQ.

## Arkitektur
Systemet er opdelt i en lagdelt struktur:

1. Models og DTOs: Sikrer separation mellem databasestruktur og API-kontrakter for at forhindre cykliske JSON-referencer.
2. Repositories: Håndterer data-adgang med optimerede LINQ-queries og brug af AsNoTracking for hurtig læsning.
3. Services: JWT Token-generering og RabbitMQ Message Producer til hardware-kommandoer.

For klassediagram se under ARCHITECTURE.md

## Sikkerhed og Autentificering
Systemet benytter en dobbelt sikkerhedsstrategi:

1. BCrypt: Ved oprettelse og login hashes adgangskoder med BCrypt, hvilket beskytter mod brute-force angreb og lækage af rå data.
2. JWT: Efter verificeret login modtager klienten et signeret token, der giver adgang til beskyttede endpoints via Authorize.

## Konfiguration
For at systemet kan køre, skal følgende værdier konfigureres i appsettings.json:

1. ConnectionStrings: Skal indeholde stien til din PostgreSQL database (DefaultConnection).
2. Jwt: Skal indeholde Key (minimum 64 tegn), Issuer og Audience.
3. RabbitMQ: Skal indeholde Host (f.eks. localhost).

## IoT Hardware Integration
API'et kommunikerer med hardwaren via RabbitMQ:

1. Start-kommando: Sender en besked til køen device_commands, som instruerer enheden i at starte måling.
2. Stop-kommando: Stopper dataindsamlingen og markerer sessionen som afsluttet i databasen.

## Test-vejledning
Vi bruger xUnit til test og har kategoriseret dem for at adskille hurtig feedback fra tung performance-test.

### Kør normale tests (Unit og Integration)
Disse tests validerer forretningslogik og database-integration mod den primære database.
```bash
dotnet test --filter "Category!=Performance"
```

#### Kør performance/stresstest på isoleret database
Denne test udfører en rå SQL-stresstest mod en dedikeret PostgreSQL-container (Port 5433). Formålet er at benchmarke hardwarens maksimale throughput og simulere den belastning, som Python-consumeren påfører systemet, uden overhead fra API eller EF Core.
```bash
dotnet test --filter "Category=Performance" --logger "console;verbosity=detailed"
```

## Swagger
Når API'et kører lokalt, kan swagger tilgåes her:
http://localhost:5193/swagger/index.html

Swagger kan også tilgås på linux serveren:
http://10.133.51.104:8081/swagger/index.html