using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Repositories;
using API.Services;
using Models;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Nødvendige værktøjer hentes udefra via interfaces
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;

    public AuthController(IUserRepository userRepo, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserRegisterDTO loginInfo)
    {
        // 1. Opslag i databasen efter brugeren via deres unikke brugernavn
        var user = await _userRepo.GetByUsernameAsync(loginInfo.Username);

        // 2. Sikkerhedstjek: Validering af brugerens eksistens og match af password-hash.
        // BCrypt verificerer det indtastede password mod den krypterede hash i databasen.
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginInfo.Password, user.PasswordHash))
        {
            // Retur af fejlkode 401 (Unauthorized) hvis legitimationsoplysninger fejler
            return Unauthorized("Ugyldigt brugernavn eller adgangskode");
        }

        // 3. Konvertering af rå bruger-entitet til en slank DTO (Data Transfer Object)
        // Kun nødvendige data som ID og rolle overføres – PasswordHash udelades altid.
        var userDto = new UserDTO 
        { 
            Id = user.Id, 
            Username = user.Username, 
            IsAdmin = user.IsAdmin 
        };

        // 4. Generering af den digitale adgangsbillet (JWT Token) via TokenService
        var token = _tokenService.CreateToken(userDto);

        // Afsendelse af token og brugernavn retur til klienten
        return Ok(new 
        { 
            Token = token, 
            Username = user.Username 
        });
    }
}