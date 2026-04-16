using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using API.Repositories;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceRepository _repo;
    
    // Dependency Injection: Interface modtages og gemmes til brug i metoderne
    public DeviceController(IDeviceRepository repo) => _repo = repo;

    // Henter alle registrerede enheder i systemet som en liste af DTO'er
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceDTO>>> GetAll()
    {
        var devices = await _repo.GetAllAsync();
        return Ok(devices);
    }

    // Opslag af en specifik enhed ud fra dens unikke database-ID
    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceDTO>> GetById(int id)
    {
        var device = await _repo.GetByIdAsync(id);
        
        // Returnerer 404 (Not Found) hvis enheden ikke eksisterer
        if (device == null) return NotFound();
        
        return Ok(device);
    }

    // Henter enhed tilknyttet en bestemt bruger. Relevante data hentes via UserId
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<DeviceDTO>> GetByUserId(int userId)
    {
        var device = await _repo.GetByUserIdAsync(userId);
        
        if (device == null) return NotFound();
        
        return Ok(device);
    }

    // Sletning af en ressource. Returnerer 204 (NoContent) ved succesfuld fjernelse
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => 
        await _repo.DeleteAsync(id) ? NoContent() : NotFound();
}