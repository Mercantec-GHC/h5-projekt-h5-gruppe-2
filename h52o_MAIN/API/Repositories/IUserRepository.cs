using Models;

namespace API.Repositories;

public interface IUserRepository
{
    // Henter alle brugere som DTO'er til oversigter
    Task<IEnumerable<UserDTO>> GetAllAsync();

    // Henter en specifik bruger som DTO baseret på ID
    Task<UserDTO?> GetByIdAsync(int id);

    // Henter den fulde User-entitet (inklusiv PasswordHash) til login-validering
    Task<User?> GetByUsernameAsync(string username);

    // Opretter en bruger og tager imod det rå password til BCrypt-hashing
    Task<User> CreateAsync(User user, string password);

    // Sletter en bruger og returnerer succes/fejl
    Task<bool> DeleteAsync(int id);
}