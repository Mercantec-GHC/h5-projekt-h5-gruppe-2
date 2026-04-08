using System;
using h5_2o_MAIN.Models;

namespace h5_2o_MAIN.Repositories;

public interface IMeasurementRepository
{
    Task<Measurement> CreateAsync(Measurement measurement);
    Task DeleteAsync(int id);
}
