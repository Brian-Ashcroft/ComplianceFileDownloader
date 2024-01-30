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


////var doctypes = new List<int>
////{
////	 1, 3, 4, 8, 9, 10, 11, 12, 15, 18,
////	 19, 20, 22, 23, 24, 27, 31, 37, 40, 41,
////	 42, 43, 44, 45, 48, 50, 51, 52, 53, 54,
////	 55, 56, 57, 58, 59, 60, 61, 62, 63, 64,
////	 66, 67, 68, 69, 70, 71, 72, 73, 74, 75
////};

////var doctypes = new List<int>
////{
////	76, 77, 78 , 79, 80, 81, 83, 84, 85, 86,
////	87, 88, 89, 90, 82, 93, 154, 214, 217, 219,
////	225, 232, 233, 234, 256, 257, 277, 293, 294, 325,
////	440, 532, 554, 592, 605, 606, 648, 649, 674, 739,
////	925, 928, 939, 940, 941, 942, 943, 947, 972, 1043
////};

////var doctypes = new List<int>
////{
////	1855, 2074, 2194, 2205, 2252, 2275, 2294, 2327, 2331, 2332,
////	2343, 2356, 2357, 2358, 2516, 2517, 2528, 2554, 2555, 2569,
////	2789, 2887, 2947, 3010, 3054, 3055, 3170, 3296, 3371, 3377,
////	3423, 3466, 3665, 4101, 4102, 4122, 4560, 5118, 5603, 5944,
////	6140, 6204, 6205, 6206, 6207, 6208, 6736, 7117, 7513, 7514
////};

////var doctypes = new List<int>
////{
////	8559, 10454, 10481, 10594, 13868, 14642, 14676, 14763, 14998, 15002,
////	16640, 18772, 18923, 18966, 18985, 18988, 18989, 18990, 18991, 19376,
////	20168, 20512, 20513, 20953, 22249, 22554, 24002, 24206, 24415, 25382,
////	25383, 25384, 25699, 27718, 31196, 31890, 32403, 32404, 32680, 32825,
////	33449, 33450, 33451, 33452, 33453, 33454, 33455, 33456, 34240, 35145
////};

var doctypes = new List<int>
{
	35489, 37456, 41828, 57729, 58129, 59087, 59712, 60589, 62186, 62187,
	62188, 62189, 62190, 62581
};


foreach (int i in doctypes)
{
    await new NonOCRDocDownloader(blobConfig, i).Execute();
}



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
//await new DocDownloader(blobConfig, new DocDownloadConfig(34240, "CovidExemptionReligious", 100, 25, 25)).Execute();




