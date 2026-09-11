using CatFact.API.DTOs;

namespace CatFact.API.Clients
{
    public class FactResponseClient(HttpClient httpClient) : IFactResponseClient
    {
        public async Task<FactResponse> GetFactResponseAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetFromJsonAsync<FactResponse>("fact", cancellationToken);
            return response ?? new FactResponse();
        }
    }
}
