
namespace CatFact.API.Infrastructure
{
    public interface IFileWriteLockManager
    {
        SemaphoreSlim Lock { get; }

        void Dispose();
    }
}