using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shiron.Mutils.API.DTos.API.Services;
using Shiron.Mutils.API.DTos.Core.Configuration;
using Shiron.Mutils.API.DTos.DB;
using Shiron.Mutils.DB.Schema;

namespace Shiron.Mutils.API.DTos.Api.Services;

public class ImageProcessingService : BackgroundService {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ImageProcessingService> _logger;
    private readonly StorageOptions _storageOptions;
    private readonly int _maxRetries = 3;
    private readonly SemaphoreSlim _fetchLimiter = new(1, 1);
    private DateTime _lastFetchTime = DateTime.MinValue;

    private static readonly TimeSpan InitialPollingInterval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan MaxPollingInterval = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan PollingBackoffStep = TimeSpan.FromSeconds(5);

    public ImageProcessingService(
        IServiceProvider serviceProvider,
        ILogger<ImageProcessingService> logger,
        IOptions<StorageOptions> storageOptions) {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _storageOptions = storageOptions.Value;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Image processing service started");

        var currentInterval = InitialPollingInterval;

        while (!stoppingToken.IsCancellationRequested) {
            try {
                var jobsProcessed = await ProcessPendingJobsAsync(stoppingToken);
                currentInterval = jobsProcessed > 0
                    ? InitialPollingInterval
                    : TimeSpan.FromTicks(Math.Min(
                        (currentInterval + PollingBackoffStep).Ticks,
                        MaxPollingInterval.Ticks));
            } catch (Exception ex) {
                _logger.LogError(ex, "Error in image processing service");
            }

            await Task.Delay(currentInterval, stoppingToken);
        }

        _logger.LogInformation("Image processing service stopped");
    }

    private async Task<int> ProcessPendingJobsAsync(CancellationToken cancellationToken) {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MutilsDbContext>();

        var pendingJobs = await dbContext.ImageJobs
            .Include(j => j.Character)
            .Where(j => j.Status == ImageJobStatus.Pending && j.RetryCount < _maxRetries)
            .OrderBy(j => j.CreatedAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        if (pendingJobs.Count == 0)
            return 0;

        _logger.LogInformation("Processing {Count} image jobs", pendingJobs.Count);

        var jobIds = pendingJobs.Select(j => j.Id).ToList();

        await Parallel.ForEachAsync(jobIds, new ParallelOptions {
            MaxDegreeOfParallelism = pendingJobs.Count,
            CancellationToken = cancellationToken
        }, async (jobId, ct) => {
            using var jobScope = _serviceProvider.CreateScope();
            var jobDbContext = jobScope.ServiceProvider.GetRequiredService<MutilsDbContext>();
            var jobStorageService = jobScope.ServiceProvider.GetRequiredService<IStorageService>();

            var job = await jobDbContext.ImageJobs
                .Include(j => j.Character)
                .FirstAsync(j => j.Id == jobId, ct);

            await ProcessJobAsync(jobDbContext, jobStorageService, job, _storageOptions.BucketAssets, ct);
        });

        return pendingJobs.Count;
    }

    private async Task ProcessJobAsync(
        MutilsDbContext dbContext,
        IStorageService storageService,
        ImageJob job,
        string bucketName,
        CancellationToken cancellationToken) {
        string? storedObjectKey = null;

        try {
            job.Status = ImageJobStatus.Processing;
            await dbContext.SaveChangesAsync(cancellationToken);

            var existingImage = await dbContext.StoredImages
                .FirstOrDefaultAsync(s => s.OriginalUrl == job.OriginalUrl, cancellationToken);

            if (existingImage is not null) {
                job.Character.StoredImageId = existingImage.Id;
                job.Status = ImageJobStatus.Completed;
                job.ProcessedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Reused existing image for character {CharacterId}", job.CharacterId);
                return;
            }

            await WaitForFetchRateLimitAsync(cancellationToken);
            var storedImage = await storageService.StoreImageAsync(job.OriginalUrl, bucketName, cancellationToken);

            if (storedImage is null) {
                throw new Exception("Failed to store image");
            }

            storedObjectKey = storedImage.ObjectKey;

            dbContext.StoredImages.Add(storedImage);
            await dbContext.SaveChangesAsync(cancellationToken);

            try {
                job.Character.StoredImageId = storedImage.Id;
                job.Status = ImageJobStatus.Completed;
                job.ProcessedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Stored image for character {CharacterId}: {ObjectKey}", job.CharacterId, storedImage.ObjectKey);
            } catch (Exception dbEx) when (storedObjectKey is not null) {
                await CleanupOrphanedObjectAsync(storageService, bucketName, storedObjectKey, dbEx);
                throw;
            }
        } catch (Exception ex) {
            job.RetryCount++;
            job.ErrorMessage = ex.Message?.Length > 500 ? ex.Message[..500] : ex.Message;
            job.Status = job.RetryCount >= _maxRetries ? ImageJobStatus.Failed : ImageJobStatus.Pending;
            await dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogWarning(ex, "Failed to process image job {JobId} (retry {RetryCount}/{MaxRetries})", job.Id, job.RetryCount, _maxRetries);
        }
    }

    private async Task WaitForFetchRateLimitAsync(CancellationToken cancellationToken) {
        await _fetchLimiter.WaitAsync(cancellationToken);
        try {
            var now = DateTime.UtcNow;
            var elapsed = now - _lastFetchTime;
            if (elapsed < TimeSpan.FromSeconds(1)) {
                await Task.Delay(TimeSpan.FromSeconds(1) - elapsed, cancellationToken);
            }
            _lastFetchTime = DateTime.UtcNow;
        } finally {
            _fetchLimiter.Release();
        }
    }

    private async Task CleanupOrphanedObjectAsync(
        IStorageService storageService,
        string bucketName,
        string objectKey,
        Exception triggerException) {
        try {
            await storageService.DeleteImageAsync(bucketName, objectKey);
            _logger.LogWarning(triggerException,
                "Cleaned up orphaned MinIO object {ObjectKey} after DB failure", objectKey);
        } catch (Exception cleanupEx) {
            _logger.LogError(cleanupEx,
                "Failed to clean up orphaned MinIO object {ObjectKey}", objectKey);
        }
    }
}
