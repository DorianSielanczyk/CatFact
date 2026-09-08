using CatFact.API.DTOs;

namespace CatFact.API.Services
{
    public interface IFactResponseService
    {
        Task<FactResponse> SaveToFileFactResponseAsync(CancellationToken cancellationToken = default);
    }
}