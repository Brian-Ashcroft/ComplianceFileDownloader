// See https://aka.ms/new-console-template for more information
using ComplianceFileDownloader;
using System.Configuration;

var userName = ConfigurationSettings.AppSettings.Get("username");
var password = ConfigurationSettings.AppSettings.Get("password");
var baseUrl = ConfigurationSettings.AppSettings.Get("baseUrl");
var connectionString = ConfigurationSettings.AppSettings.Get("connectionString");

//var exampleDownloader = new ExampleDownloader(baseUrl, userName, password, connectionString);
//await exampleDownloader.Execute();
//var covidDownloader = new CovidDownloader(baseUrl, userName, password, connectionString);

var blsDownloader = new BlsDownloader(baseUrl, userName, password, connectionString);
await blsDownloader.Execute();

