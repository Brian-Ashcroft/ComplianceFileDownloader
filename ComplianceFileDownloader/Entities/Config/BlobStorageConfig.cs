namespace ComplianceFileDownloader.Entities.Config;

internal record struct BlobStorageConfig
{
    public string BaseUrl { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ConnectionString { get; set; }
}