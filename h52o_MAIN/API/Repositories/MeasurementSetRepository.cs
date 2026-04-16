using Microsoft.EntityFrameworkCore;
using Models;
using API.Data;
using API.Services;

namespace API.Repositories;


public class MeasurementSetRepository : IMeasurementSetRepository
{
    private readonly AppDbContext _context;
    private readonly IMessageProducer _producer;

    // Afhængigheder (database og beskedkø) modtages udefra via constructor injection
    public MeasurementSetRepository(AppDbContext context, IMessageProducer producer)
    {
        _context = context;
        _producer = producer;
    }

    // Opretter en ny måle-session. Tjekker først for eksisterende aktive sæt for at undgå duplikater.
    public async Task<MeasurementSet> StartNewSetAsync(int deviceId, string location)
    {
        var existingActiveSet = await _context.MeasurementSets
            .FirstOrDefaultAsync(s => s.DeviceId == deviceId && s.EndTime == null);

        // Hvis der allerede findes et aktivt sæt, returneres dette i stedet for at oprette et nyt
        if (existingActiveSet != null) return existingActiveSet;

        var newSet = new MeasurementSet
        {
            DeviceId = deviceId,
            Location = location,
            StartTime = DateTime.UtcNow
        };

        _context.MeasurementSets.Add(newSet);
        
        // Enhedens status opdateres for at afspejle den igangværende måling
        var device = await _context.Devices.FindAsync(deviceId);
        if (device != null) device.IsActive = true;

        await _context.SaveChangesAsync();
        
        // Signal sendes til hardware via RabbitMQ om at påbegynde dataindsamling
        await _producer.SendStartCommand(deviceId, newSet.Id);

        return newSet;
    }

    // Afslutter en igangværende session ved at sætte slut-tidspunkt og deaktivere enheden.
    public async Task<bool> StopSetAsync(int setId)
    {
        var set = await _context.MeasurementSets.FindAsync(setId);
        if (set == null || set.EndTime != null) return false;

        set.EndTime = DateTime.UtcNow;
        
        var device = await _context.Devices.FindAsync(set.DeviceId);
        if (device != null) device.IsActive = false;

        // Gemmer ændringer og returnerer true hvis operationen lykkedes
        return await _context.SaveChangesAsync() > 0;
    }

    // Henter historiske målesæt tilknyttet en bruger.
    public async Task<IEnumerable<UserMeasurementSetDTO>> GetSetsByUserIdAsync(int userId)
    {
        return await _context.MeasurementSets
            .Where(s => s.Device.UserId == userId)
            .OrderByDescending(s => s.StartTime)
            .Select(s => new UserMeasurementSetDTO
            {
                Id = s.Id,
                Location = s.Location,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            })
            .AsNoTracking()
            .ToListAsync();
    }

    // Henter detaljeret information om et specifikt sæt, herunder en liste over tilhørende måle-ID'er.
    public async Task<MeasurementSetDTO?> GetSetWithMeasurementsAsync(int setId)
    {
        return await _context.MeasurementSets
            .Where(s => s.Id == setId)
            .Select(s => new MeasurementSetDTO
            {
                Id = s.Id,
                Location = s.Location,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                DeviceId = s.DeviceId,
                MeasurementIds = s.Measurements.Select(m => m.Id).ToList()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}
