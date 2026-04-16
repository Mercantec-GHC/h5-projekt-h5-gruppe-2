using Models;

namespace API.Repositories;

public interface IMeasurementSetRepository
{
    // Opretter et sæt
    Task<MeasurementSet> StartNewSetAsync(int deviceId, string location);

    // Stopper et aktivt sæt
    Task<bool> StopSetAsync(int setId);

    // Henter alle sæt tilhørende en bruger uden ID-lister
    Task<IEnumerable<UserMeasurementSetDTO>> GetSetsByUserIdAsync(int userId);

    // Henter ét specifikt sæt inklusiv Measurement ID-liste
    Task<MeasurementSetDTO?> GetSetWithMeasurementsAsync(int setId);
}