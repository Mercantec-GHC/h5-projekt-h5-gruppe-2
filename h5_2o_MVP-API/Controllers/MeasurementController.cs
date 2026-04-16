using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using h5_2o_MAIN.Data;
using h5_2o_MAIN.Models;

namespace h5_2o_MAIN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MeasurementController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Measurement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Measurement>>> GetMeasurements()
        {
            return await _context.Measurements.ToListAsync();
        }

        // GET: api/Measurement/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Measurement>> GetMeasurement(int id)
        {
            var measurement = await _context.Measurements.FindAsync(id);

            if (measurement == null)
            {
                return NotFound();
            }

            return measurement;
        }

        private bool MeasurementExists(int id)
        {
            return _context.Measurements.Any(e => e.id == id);
        }
    }
}
