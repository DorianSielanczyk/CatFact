namespace CatFact.API.Models
{
    public class FactApiOptions
    {
        public const string SectionName = "CatFactApi";

        public required string BaseUrl { get; init; }
        public int TimeoutSeconds { get; init; } = 5;
        public int TotalTimeoutSeconds { get; init; } = 20;
        public int MaxRetryAttempts { get; init; } = 3;
    }
}
