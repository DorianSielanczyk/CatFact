namespace CatFact.API.DTOs
{
    public record FactResponse
    {
        public string Fact { get; init; } = string.Empty;
        public int Length { get; init; }
    }
}
