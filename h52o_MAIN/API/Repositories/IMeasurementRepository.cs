using Models;

namespace API.Repositories;

public interface IMeasurementRepository
{
    // Henter en specifik måling
    Task<MeasurementDTO?> GetByIdAsync(int id);

    // Henter data fra et tidspunkt og frem for en bestemt bruger (Realtid)
    Task<IEnumerable<MeasurementDTO>> GetByUserIdFromDateAsync(int userId, DateTime since);

    // Henter data i et tidsinterval for en bestemt bruger (Historik)
    Task<IEnumerable<MeasurementDTO>> GetByUserIdInIntervalAsync(int userId, DateTime anchorDate, TimeSpan interval);

    // Henter alt data for et specifikt målesæt
    Task<IEnumerable<MeasurementDTO>> GetBySetIdAsync(int setId);
}