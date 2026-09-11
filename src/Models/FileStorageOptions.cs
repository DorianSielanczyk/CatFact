using System.ComponentModel.DataAnnotations;

namespace CatFact.API.Models
{
    public class FileStorageOptions
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)] // 5 MB
        public long MaxFileSizeBytes { get; init; } = 5 * 1024 * 1024;

        [Range(1, 100)]
        public int MaxConcurrentWrites { get; init; } = 1;
    }
}
