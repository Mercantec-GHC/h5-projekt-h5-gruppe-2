using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    // Konfiguration hentes for at få adgang til appsettings.json
    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    // Genererer en signeret streng, der fungerer som brugerens digitale identitet
    public string CreateToken(UserDTO user)
    {
        // Definition af brugerens rettigheder og identitet i tokenet
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
        };

        // Hentning af den hemmelige nøgle og opsætning af krypteringsalgoritme
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // Konfiguration af tokenets levetid, modtager og udsteder
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7), // Tokenet er gyldigt i 7 dage
            SigningCredentials = creds,
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"]
        };

        // Selve dannelsen og skrivningen af JWT-strengen
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}