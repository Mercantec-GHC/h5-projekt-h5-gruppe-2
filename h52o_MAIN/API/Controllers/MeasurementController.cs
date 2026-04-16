using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using API.Repositories;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementController : ControllerBase
{
    private readonly IMeasurementRepository _repo;

    // Interface modtages via Dependency Injection for at sikre løs kobling til datalaget
    public MeasurementController(IMeasurementRepository repo)
    {
        _repo = repo;
    }

    // Realtids-opslag: Henter alle nye målinger for en bruger siden et specifikt tidspunkt.
    // Anvendes typisk af frontend-klienter til løbende opdatering af grafer.
    [HttpGet("user/{userId}/since")]
    public async Task<ActionResult<IEnumerable<MeasurementDTO>>> GetSince(int userId, [FromQuery] DateTime since)
    {
        var data = await _repo.GetByUserIdFromDateAsync(userId, since);
        return Ok(data);
    }

    // Historik-opslag: Henter måledata i et defineret interval bagud i tid fra et ankerpunkt.
    // Gør det muligt for brugeren at navigere i historiske log-filer og trends.
    [HttpGet("user/{userId}/history")]
    public async Task<ActionResult<IEnumerable<MeasurementDTO>>> GetHistory(
        int userId, 
        [FromQuery] DateTime anchor, 
        [FromQuery] int minutes)
    {
        var interval = TimeSpan.FromMinutes(minutes);
        var data = await _repo.GetByUserIdInIntervalAsync(userId, anchor, interval);
        return Ok(data);
    }

    // Sessions-opslag: Returnerer samtlige målinger tilhørende et specifikt målesæt.
    // Bruges til detaljeret analyse af en afsluttet måle-session.
    [HttpGet("set/{setId}")]
    public async Task<ActionResult<IEnumerable<MeasurementDTO>>> GetBySet(int setId)
    {
        var data = await _repo.GetBySetIdAsync(setId);
        return Ok(data);
    }

    // Opslag af enkeltstående målepunkt via dets unikke database-ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<MeasurementDTO>> GetById(int id)
    {
        var measurement = await _repo.GetByIdAsync(id);
        if (measurement == null) return NotFound();
        return Ok(measurement);
    }
}