using Models;

namespace API.Repositories;

public interface IDeviceRepository
{
    Task<IEnumerable<DeviceDTO>> GetAllAsync();

    Task<DeviceDTO?> GetByIdAsync(int id);

    Task<DeviceDTO?> GetByUserIdAsync(int userId);

    Task<bool> DeleteAsync(int id);
}