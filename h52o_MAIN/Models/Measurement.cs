using System;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Models;

public class Measurement
{
    public int Id { get; set; }
    public int Value { get; set; }
    public DateTime Timestamp { get; set; } 
    public int MeasurementSetId { get; set; }
    public MeasurementSet MeasurementSet { get; set; } = null!;
}
