namespace Models;

public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public int? DeviceId { get; set; }
    public bool IsDeviceActive { get; set; }
    public List<int> MeasurementSetIds { get; set; } = new();
}