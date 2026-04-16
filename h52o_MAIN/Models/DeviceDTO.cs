namespace Models;

public class DeviceDTO
{
    public int Id { get; set; }
    public string DeviceKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int UserId { get; set; }
    
    // Vi inkluderer kun ID'erne på de tilhørende målesæt
    // ligesom vi gjorde i UserDTO for at holde den strømlinet.
    public List<int> MeasurementSetIds { get; set; } = new();
}