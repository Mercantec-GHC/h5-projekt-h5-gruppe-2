using Microsoft.EntityFrameworkCore;
using Models;
using API.Data;
using BCrypt.Net;

namespace API.Repositories;

public class UserRepository : IUserRepository 
{
    private readonly AppDbContext _db;
    
    // Database-context for adgang til brugerdata
    public UserRepository(AppDbContext db) => _db = db;

    // Henter samtlige brugere og pakker dem ind i DTO-format for sikker overførsel
    public async Task<IEnumerable<UserDTO>> GetAllAsync() =>
        await _db.Users
            .Include(u => u.Device)
            .ThenInclude(d => d!.Sets)
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                IsAdmin = u.IsAdmin,
                DeviceId = u.Device != null ? u.Device.Id : null,
                IsDeviceActive = u.Device != null && u.Device.IsActive,
                // Transformation til en simpel liste af ID'er
                MeasurementSetIds = u.Device != null 
                    ? u.Device.Sets.Select(s => s.Id).ToList() 
                    : new List<int>()
            })
            .AsNoTracking()
            .ToListAsync();

    // Opslag af en specifik bruger baseret på ID
    public async Task<UserDTO?> GetByIdAsync(int id) =>
        await _db.Users
            .Include(u => u.Device)
                .ThenInclude(d => d!.Sets)
            .Where(u => u.Id == id)
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                IsAdmin = u.IsAdmin,
                DeviceId = u.Device != null ? u.Device.Id : null,
                IsDeviceActive = u.Device != null && u.Device.IsActive,
                MeasurementSetIds = u.Device != null 
                    ? u.Device.Sets.Select(s => s.Id).ToList() 
                    : new List<int>()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

    // Henter den fulde entitet inklusiv PasswordHash.
    // Denne metode returnerer aldrig data direkte til API-endpoints.
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    // Oprettelse af ny bruger.
    public async Task<User> CreateAsync(User user, string password) 
    { 
        // BCrypt genererer en unik salt og hash for adgangskoden
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        
        _db.Users.Add(user); 
        await _db.SaveChangesAsync(); 
        return user; 
    }

    // Fjernelse af brugerressource. Databasen sikrer sletning af tilhørende data.
    public async Task<bool> DeleteAsync(int id) 
    {
        var u = await _db.Users.FindAsync(id);
        if (u == null) return false;
        
        _db.Users.Remove(u); 
        return await _db.SaveChangesAsync() > 0;
    }
}