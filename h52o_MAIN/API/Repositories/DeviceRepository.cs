using Microsoft.EntityFrameworkCore;
using Models;
using API.Data;

namespace API.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _db;
    public DeviceRepository(AppDbContext db) => _db = db;

    // Henter alle enheder som DTO'er
    public async Task<IEnumerable<DeviceDTO>> GetAllAsync() => 
        await _db.Devices
            .Select(d => new DeviceDTO 
            {
                Id = d.Id,
                DeviceKey = d.DeviceKey,
                IsActive = d.IsActive,
                UserId = d.UserId,
                MeasurementSetIds = d.Sets.Select(s => s.Id).ToList()
            })
            .AsNoTracking()
            .ToListAsync();

    // Henter en specifik enhed baseret på ID
    public async Task<DeviceDTO?> GetByIdAsync(int id) => 
        await _db.Devices
            .Where(d => d.Id == id)
            .Select(d => new DeviceDTO 
            {
                Id = d.Id,
                DeviceKey = d.DeviceKey,
                IsActive = d.IsActive,
                UserId = d.UserId,
                MeasurementSetIds = d.Sets.Select(s => s.Id).ToList()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

    // Henter enhed baseret på UserId
    public async Task<DeviceDTO?> GetByUserIdAsync(int userId) =>
        await _db.Devices
            .Where(d => d.UserId == userId)
            .Select(d => new DeviceDTO 
            {
                Id = d.Id,
                DeviceKey = d.DeviceKey,
                IsActive = d.IsActive,
                UserId = d.UserId,
                MeasurementSetIds = d.Sets.Select(s => s.Id).ToList()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

    // Sletning forbliver bool-baseret
    public async Task<bool> DeleteAsync(int id)
    {
        var device = await _db.Devices.FindAsync(id);
        if (device == null) return false;

        _db.Devices.Remove(device);
        return await _db.SaveChangesAsync() > 0;
    }
}