namespace Models;

/* 
 * TEORI: DATA SHAPING & LOAD OPTIMIZATION
 * 
 * 1. Data Shaping: Denne DTO er designet til at give et detaljeret indblik i en 
 *    målesession uden at belaste netværket med tunge objekter. Ved kun at inkludere 
 *    en liste af ID'er (MeasurementIds) i stedet for de fulde målinger, bevares 
 *    kontrollen over datamængden.
 * 
 * 2. Payload Reduction: Ved at transformere komplekse 1-til-mange relationer til 
 *    simple lister af heltal, reduceres størrelsen på JSON-svaret drastisk. Dette 
 *    er særligt vigtigt i IoT-systemer, hvor et målesæt kan indeholde tusindvis 
 *    af individuelle datapunkter.
 * 
 * 3. Lazy Loading Simulation: Ved kun at levere ID'er tvinges klienten til en 
 *    "on-demand" tilgang. Klienten kan vælge kun at hente de specifikke måledata, 
 *    der er nødvendige for den aktuelle visning, hvilket sikrer en responsiv brugeroplevelse.
 */

public class MeasurementSetDTO
{
    // Unik identifikator for målesættet
    public int Id { get; set; }

    // Beskrivelse af den fysiske placering for målingen
    public string? Location { get; set; }

    // Tidspunkt for sessionens begyndelse
    public DateTime StartTime { get; set; }

    // Tidspunkt for sessionens afslutning (null hvis sessionen stadig er aktiv)
    public DateTime? EndTime { get; set; }

    // Reference til den hardware-enhed, der udfører målingen
    public int DeviceId { get; set; }
    
    // Liste af unikke ID'er på de tilhørende målinger. 
    // Dette strømlinede format minimerer hukommelsesforbruget ved dataoverførsel.
    public List<int> MeasurementIds { get; set; } = new();
}