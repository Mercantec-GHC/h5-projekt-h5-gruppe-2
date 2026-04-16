using System;

namespace Models;

public class MeasurementSet
{
    public int Id { get; set; }
    public string? Location { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    public List<Measurement> Measurements { get; set; } = new();
}
