// See https://aka.ms/new-console-template for more information
using ComplianceFileDownloader;
using ComplianceFileDownloader.Downloaders;
using System.Configuration;

var userName = ConfigurationSettings.AppSettings.Get("username");
var password = ConfigurationSettings.AppSettings.Get("password");
var baseUrl = ConfigurationSettings.AppSettings.Get("baseUrl");
var connectionString = ConfigurationSettings.AppSettings.Get("connectionString");

//var exampleDownloader = new ExampleDownloader(baseUrl, userName, password, connectionString);
//await exampleDownloader.Execute();
//var covidDownloader = new CovidDownloader(baseUrl, userName, password, connectionString);

//var aclsDownloader = new AclsDownloader(baseUrl, userName, password, connectionString);
//await aclsDownloader.Execute();

//var blsDownloader = new BlsDownloader(baseUrl, userName, password, connectionString);
//await blsDownloader.Execute();

//var covidBoosterDownloader = new CovidBoosterDownloader(baseUrl, userName, password, connectionString);
//await covidBoosterDownloader.Execute();

var covidDose1Downloader = new CovidDose1Downloader(baseUrl, userName, password, connectionString);
await covidDose1Downloader.Execute();

//var covidDose2Downloader = new CovidDose2Downloader(baseUrl, userName, password, connectionString);
//await covidDose2Downloader.Execute();

//var driversLicenseDownloader = new DriversLicenseDownloader(baseUrl, userName, password, connectionString);
//await driversLicenseDownloader.Execute();

//var palsDownloader = new PalsDownloader(baseUrl, userName, password, connectionString);
//await palsDownloader.Execute();

//var passportDownloader = new PassportDownloader(baseUrl, userName, password, connectionString);
//await passportDownloader.Execute();

//var rubellaDownloader = new RubellaDownloader(baseUrl, userName, password, connectionString);
//await rubellaDownloader.Execute();

