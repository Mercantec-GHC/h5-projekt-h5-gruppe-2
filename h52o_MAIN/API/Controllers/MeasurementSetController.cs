using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using API.Repositories;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementSetController : ControllerBase
{
    private readonly IMeasurementSetRepository _repo;

    // Interface injiceres for at abstrahere logikken for håndtering af målesæt
    public MeasurementSetController(IMeasurementSetRepository repo) => _repo = repo;

    // Aktivering af en ny målesession. Opretter entitet i databasen og sender signal til hardware.
    // Returnerer en slank DTO med metadata om det nyoprettede sæt.
    [HttpPost("start/{deviceId}")]
    public async Task<ActionResult<UserMeasurementSetDTO>> Start(int deviceId, [FromQuery] string location)
    {
        var newSet = await _repo.StartNewSetAsync(deviceId, location);
        
        // Manuel projektion til DTO sikrer, at klienten kun modtager relevant metadata
        var dto = new UserMeasurementSetDTO
        {
            Id = newSet.Id,
            Location = newSet.Location,
            StartTime = newSet.StartTime,
            EndTime = newSet.EndTime
        };

        return Ok(dto);
    }

    // Deaktivering af en igangværende session. Opdaterer slut-tidspunkt og stopper hardware-logning.
    [HttpPost("stop/{setId}")]
    public async Task<IActionResult> Stop(int setId)
    {
        var success = await _repo.StopSetAsync(setId);
        return success ? Ok() : NotFound();
    }

    // Oversigts-opslag: Henter alle historiske målesæt tilknyttet en specifik bruger.
    // Bruger det letteste DTO-format for at optimere indlæsning af lister.
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<UserMeasurementSetDTO>>> GetByUserId(int userId)
    {
        var sets = await _repo.GetSetsByUserIdAsync(userId);
        return Ok(sets);
    }

    // Detalje-opslag: Henter fuld information om et målesæt, inklusive referencer til alle målinger.
    [HttpGet("{id}")]
    public async Task<ActionResult<MeasurementSetDTO>> GetWithData(int id)
    {
        var set = await _repo.GetSetWithMeasurementsAsync(id);
        
        if (set == null) return NotFound();
        
        return Ok(set);
    }
}
