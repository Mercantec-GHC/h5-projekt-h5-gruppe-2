namespace Models;

/* 
 * TEORI: DATA TRANSFER OBJECTS (DTO) & NETWORK EFFICIENCY
 * 
 * 1. Data Decoupling: Ved at bruge en DTO adskilles API-kontrakten fra selve 
 *    database-modellen. Dette sikrer, at ændringer i databasen (f.eks. nye kolonner) 
 *    ikke automatisk tvinger klienten til at opdatere sin kode.
 * 
 * 2. Flat Data Structure: MeasurementDTO er en "flad" struktur. I stedet for at 
 *    inkludere hele MeasurementSet- og Device-objekter, inkluderes kun deres ID'er. 
 *    Dette reducerer mængden af data markant og gør det muligt at sende tusindvis 
 *    af målepunkter over netværket uden stort overhead.
 * 
 * 3. Cross-Resource Reference: Ved at inkludere DeviceId direkte i målingen, 
 *    får klienten kontekst om kilden med det samme. Dette sparer klienten for 
 *    ekstra opslag i andre tabeller for at finde ud af, hvilken hardware data stammer fra.
 */

public class MeasurementDTO
{
    // Unik identifikation af det specifikke målepunkt
    public int Id { get; set; }

    // Den rå måleværdi opsamlet af sensoren
    public int Value { get; set; }

    // Præcist tidsstempel for hvornår målingen fandt sted
    public DateTime Timestamp { get; set; }

    // Reference til den specifikke session (målesæt), målingen tilhører
    public int MeasurementSetId { get; set; }

    // Direkte reference til hardware-enhedens ID for nem identifikation i frontend
    public int DeviceId { get; set; }
}