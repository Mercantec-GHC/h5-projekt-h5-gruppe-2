using System;
using h5_2o_MAIN.Models;
using h5_2o_MAIN.Data;

namespace h5_2o_MAIN.Repositories;

public class MeasurementRepository : IMeasurementRepository
{
    private readonly AppDbContext _context;

    public MeasurementRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Measurement> CreateAsync(Measurement measurement)
    {
        measurement.timestamp = DateTime.UtcNow;
        _context.Measurements.Add(measurement);
        await _context.SaveChangesAsync();
        return measurement;
    }

    public async Task DeleteAsync(int id)
    {
        var measurement = await _context.Measurements.FindAsync(id);
        if (measurement != null)
        {
            _context.Measurements.Remove(measurement);
            await _context.SaveChangesAsync();
        }
    }
}
