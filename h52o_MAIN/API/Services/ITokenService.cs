using Models;

namespace API.Services;

public interface ITokenService
{
     // Kontrakt for generering af token
     // Modtager en UserDTO for at bygge Id, Navn og Rolle
     string CreateToken(UserDTO user); 
}