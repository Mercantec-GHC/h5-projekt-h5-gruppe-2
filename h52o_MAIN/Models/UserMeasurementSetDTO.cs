namespace Models;

/* 
 * TEORI: SPECIALIZED VIEW MODELS & PERFORMANCE OPTIMIZATION
 * 
 * 1. Specialized View Models: Denne DTO er en ekstremt forenklet udgave af et målesæt,
 *    designet specifikt til oversigtslister. Ved at fjerne alle relationelle data 
 *    (som DeviceId og MeasurementIds) opnås den mest kompakte datastruktur muligt.
 * 
 * 2. Performance Optimization: Når systemet skal vise en historik over måske 100 
 *    forskellige sessioner, vil denne DTO minimere den samlede JSON-størrelse markant. 
 *    Dette reducerer både parsing-tid i klienten og båndbreddeforbrug.
 * 
 * 3. Information Hiding (UX): Ved kun at levere overordnede metadata (Id, Location, Tid), 
 *    skabes en renere grænseflade for frontend-udvikleren, der kan fokusere på 
 *    selve listevisningen uden at forholde sig til tekniske ID-lister.
 */

public class UserMeasurementSetDTO
{
    // Unik identifikation af målesættet
    public int Id { get; set; }

    // Navn på lokationen for målingen (f.eks. "Tag" eller "Have")
    public string? Location { get; set; }

    // Tidspunkt for hvornår måle-sessionen blev startet
    public DateTime StartTime { get; set; }

    // Tidspunkt for hvornår sessionen blev afsluttet
    public DateTime? EndTime { get; set; }
}