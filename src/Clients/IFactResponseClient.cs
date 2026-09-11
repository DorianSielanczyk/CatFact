using CatFact.API.DTOs;

namespace CatFact.API.Clients
{
    public interface IFactResponseClient
    {
        Task<FactResponse> GetFactResponseAsync(CancellationToken cancellationToken = default);
    }
}
