namespace Shiron.TheArchive.API.Services;

public interface IStorageService {
    Task<Stream?> GetAsync(string bucket, string objectKey, CancellationToken cancellationToken = default);
    Task<string> StoreAsync(string bucket, string objectKey, Stream data, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string bucket, string objectKey, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string bucket, string objectKey, CancellationToken cancellationToken = default);
}
