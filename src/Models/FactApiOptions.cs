using System.ComponentModel.DataAnnotations;

namespace CatFact.API.Models 
{
    public class FactApiOptions
    {
        public const string SectionName = "CatFactApi";

        [Required(ErrorMessage = "Brak adresu BaseUrl w konfiguracji.")]
        [Url(ErrorMessage = "BaseUrl musi być prawidłowym adresem URL.")]
        public required string BaseUrl { get; init; }

        [Range(1, 60, ErrorMessage = "Timeout musi mieścić się w przedziale od 1 do 60 sekund.")]
        public int TimeoutSeconds { get; init; } = 5;

        [Range(1, 120, ErrorMessage = "Całkowity timeout musi wynosić od 1 do 120 sekund.")]
        public int TotalTimeoutSeconds { get; init; } = 20;

        [Range(0, 10, ErrorMessage = "Liczba ponownych prób (retry) musi wynosić od 0 do 10.")]
        public int MaxRetryAttempts { get; init; } = 3;
    }
}
