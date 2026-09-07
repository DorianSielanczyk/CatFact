using CatFact.API.DTOs;

namespace CatFact.API.Client
{
    public interface IFactResponseClient
    {
        Task<FactResponse> GetFactResponseAsync(CancellationToken cancellationToken = default);
    }
}