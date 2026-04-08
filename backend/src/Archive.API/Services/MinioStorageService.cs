using Minio;
using Minio.DataModel.Args;

namespace Shiron.TheArchive.API.Services;

public class MinioStorageService(IMinioClient minioClient, ILogger<MinioStorageService> logger) : IStorageService {
    private readonly IMinioClient _minioClient = minioClient;
    private readonly ILogger<MinioStorageService> _logger = logger;
    private readonly HashSet<string> _verifiedBuckets = [];

    public async Task<Stream?> GetAsync(string bucket, string objectKey, CancellationToken cancellationToken = default) {
        try {
            var memoryStream = new MemoryStream();
            var getArgs = new GetObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectKey)
                .WithCallbackStream((stream, ct) => stream.CopyToAsync(memoryStream, ct));

            await _minioClient.GetObjectAsync(getArgs, cancellationToken);
            memoryStream.Position = 0;
            return memoryStream;
        } catch (Exception ex) {
            _logger.LogError(ex, "Failed to get object {ObjectKey} from bucket {Bucket}", objectKey, bucket);
            return null;
        }
    }

    public async Task<string> StoreAsync(string bucket, string objectKey, Stream data, string contentType, CancellationToken cancellationToken = default) {
        try {
            await EnsureBucketExistsAsync(bucket, cancellationToken);

            var putArgs = new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectKey)
                .WithStreamData(data)
                .WithObjectSize(data.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putArgs, cancellationToken);
            _logger.LogInformation("Stored object {ObjectKey} in bucket {Bucket}", objectKey, bucket);
            return objectKey;
        } catch (Exception ex) {
            _logger.LogError(ex, "Failed to store object {ObjectKey} in bucket {Bucket}", objectKey, bucket);
            throw;
        }
    }

    public async Task DeleteAsync(string bucket, string objectKey, CancellationToken cancellationToken = default) {
        try {
            var removeArgs = new RemoveObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectKey);

            await _minioClient.RemoveObjectAsync(removeArgs, cancellationToken);
            _logger.LogInformation("Deleted object {ObjectKey} from bucket {Bucket}", objectKey, bucket);
        } catch (Exception ex) {
            _logger.LogError(ex, "Failed to delete object {ObjectKey} from bucket {Bucket}", objectKey, bucket);
        }
    }

    public async Task<bool> ExistsAsync(string bucket, string objectKey, CancellationToken cancellationToken = default) {
        try {
            var statArgs = new StatObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectKey);

            await _minioClient.StatObjectAsync(statArgs, cancellationToken);
            return true;
        } catch {
            return false;
        }
    }

    private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken) {
        if (_verifiedBuckets.Contains(bucketName))
            return;

        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

        if (!exists) {
            _logger.LogInformation("Creating MinIO bucket {BucketName}", bucketName);
            var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        _verifiedBuckets.Add(bucketName);
    }
}
