namespace Models;

/* 
 * TEORI: INPUT VALIDATION & SECURITY CONTRACTS
 * 
 * 1. Security Contract: Denne DTO fungerer som en sikker "indgangs-kontrakt" for 
 *    nye brugere. Ved at kræve et råt password her, sikres det, at API'et modtager 
 *    de nødvendige informationer til at udføre en sikker BCrypt-hashing i datalaget.
 * 
 * 2. Separation of Concerns: UserRegisterDTO adskiller sig fra UserDTO ved at 
 *    inkludere et Password-felt, men udelukke database-ID og relationer. Dette 
 *    sikrer, at klienten kun sender data, der er relevante for selve oprettelsesprocessen.
 * 
 * 3. Overposting Protection: Ved kun at definere specifikke felter (Username, Password, IsAdmin) 
 *    forhindres hackere i at "indsprøjte" data i uønskede felter i databasen, 
 *    hvilket er en kritisk sikkerhedsforanstaltning i moderne Web API'er.
 */

public class UserRegisterDTO
{
    // Brugernavn valgt af klienten ved oprettelse
    public string Username { get; set; } = string.Empty;

    // Det rå password som sendes sikkert via HTTPS til serveren for hashing
    public string Password { get; set; } = string.Empty;

    // Angiver om den nye bruger skal oprettes med administrative rettigheder
    public bool IsAdmin { get; set; }
}