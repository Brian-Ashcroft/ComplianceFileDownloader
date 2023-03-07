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

//await new DocDownloader(blobConfig, new DocDownloadConfig(30, "NegTbOrChestXray", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(35, "HepatitisB", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(33, "Mumps", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(32, "Rubeola", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(34, "Rubella", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(36, "Varicella", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(257, "VoidedCheck", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(7, "SocialSecurity", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(29, "PhysicalExam", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(92, "FitMask", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(605, "TranscriptOrDiploma", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(2206, "NameChange", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(972, "FingerPrinting", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(52, "StateLicense", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(26, "ElectronicFetalMonitoring", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(27, "TechCert", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(47236, "CovidBooster2", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(2538, "NIHStrokeCert", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(17, "NRP", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(25, "TNCC", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(3422, "CPI", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(91, "I9SupportingDoc", 100, 25, 25)).Execute();
//await new DocDownloader(blobConfig, new DocDownloadConfig(21, "CHEMO", 100, 25, 25)).Execute();

await new DocDownloader(blobConfig, new DocDownloadConfig(34240, "CovidExemptionReligious", 100, 25, 25)).Execute();
