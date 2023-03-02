// See https://aka.ms/new-console-template for more information
using ComplianceFileDownloader;
using ComplianceFileDownloader.Downloaders;
using ComplianceFileDownloader.Entities.Config;
using System.Configuration;

var blobConfig = new BlobStorageConfig()
{
	BaseUrl = ConfigurationSettings.AppSettings.Get("baseUrl"),
	UserName = ConfigurationSettings.AppSettings.Get("username"),
	Password = ConfigurationSettings.AppSettings.Get("password"),
	ConnectionString = ConfigurationSettings.AppSettings.Get("connectionString")
};

await new DocDownloader(blobConfig, new DocDownloadConfig(14, "BLS", 1, 0, 0)).Execute();
await new DocDownloader(blobConfig, new DocDownloadConfig(14, "BLS", 1, 0, 0)).Execute();

