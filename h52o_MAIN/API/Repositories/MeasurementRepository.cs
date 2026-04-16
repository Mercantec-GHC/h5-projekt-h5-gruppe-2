using Microsoft.EntityFrameworkCore;
using Models;
using API.Data;

namespace API.Repositories;

public class MeasurementRepository : IMeasurementRepository
{
    private readonly AppDbContext _context;
    
    // Database-context for at give adgang til måledata
    public MeasurementRepository(AppDbContext context) => _context = context;

    // Opslag af en specifik måling ud fra dens unikke ID
    public async Task<MeasurementDTO?> GetByIdAsync(int id)
    {
        return await _context.Measurements
            .Where(m => m.Id == id)
            .Select(m => new MeasurementDTO 
            {
                Id = m.Id,
                Value = m.Value,
                Timestamp = m.Timestamp,
                MeasurementSetId = m.MeasurementSetId,
                // Kryds-reference til DeviceId via relationen til MeasurementSet
                DeviceId = m.MeasurementSet.DeviceId
            })
            .FirstOrDefaultAsync();
    }

    // Realtids-forespørgsel: Henter alle målinger efter et bestemt tidspunkt for en bruger.
    // OrderBy sikrer, at data leveres i korrekt tidsmæssig rækkefølge til grafer.
    public async Task<IEnumerable<MeasurementDTO>> GetByUserIdFromDateAsync(int userId, DateTime since)
    {
        return await _context.Measurements
            .Where(m => m.MeasurementSet.Device.UserId == userId && m.Timestamp >= since)
            .OrderBy(m => m.Timestamp)
            .Select(m => new MeasurementDTO
            {
                Id = m.Id,
                Value = m.Value,
                Timestamp = m.Timestamp,
                MeasurementSetId = m.MeasurementSetId,
                DeviceId = m.MeasurementSet.DeviceId
            })
            .AsNoTracking()
            .ToListAsync();
    }

    // Historik-forespørgsel: Udtrækker data i et specifikt tidsinterval.
    public async Task<IEnumerable<MeasurementDTO>> GetByUserIdInIntervalAsync(int userId, DateTime anchorDate, TimeSpan interval)
    {
        var start = anchorDate.Subtract(interval);
        
        return await _context.Measurements
            .Where(m => m.MeasurementSet.Device.UserId == userId 
                   && m.Timestamp >= start 
                   && m.Timestamp <= anchorDate)
            .OrderBy(m => m.Timestamp)
            .Select(m => new MeasurementDTO
            {
                Id = m.Id,
                Value = m.Value,
                Timestamp = m.Timestamp,
                MeasurementSetId = m.MeasurementSetId,
                DeviceId = m.MeasurementSet.DeviceId
            })
            .AsNoTracking()
            .ToListAsync();
    }

    // Henter samtlige målinger tilknyttet et specifikt MeasurementSet.
    public async Task<IEnumerable<MeasurementDTO>> GetBySetIdAsync(int setId)
    {
        return await _context.Measurements
            .Where(m => m.MeasurementSetId == setId)
            .OrderBy(m => m.Timestamp)
            .Select(m => new MeasurementDTO
            {
                Id = m.Id,
                Value = m.Value,
                Timestamp = m.Timestamp,
                MeasurementSetId = m.MeasurementSetId,
                DeviceId = m.MeasurementSet.DeviceId
            })
            .AsNoTracking()
            .ToListAsync();
    }
}