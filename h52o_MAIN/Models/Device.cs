using System;

namespace Models;

public class Device
{
    public int Id { get; set; }
    public string DeviceKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int UserId { get; set; }
    public List<MeasurementSet> Sets { get; set; } = new();
}