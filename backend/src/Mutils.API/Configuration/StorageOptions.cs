namespace Shiron.Mutils.API.Configuration;

public sealed class StorageOptions {
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = "minioadmin";
    public string SecretKey { get; set; } = "minioadmin";
    public bool UseSsl { get; set; }
    public string BucketAssets { get; set; } = "mutils-assets";
    public string BucketUserData { get; set; } = "mutils-user-data";
}
