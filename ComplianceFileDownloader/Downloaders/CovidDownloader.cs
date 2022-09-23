using ComplianceFileDownloader.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplianceFileDownloader.Downloaders;
internal class CovidDownloader
{
    private string baseUrl;
    private string userName;
    private string password;
    private string connectionString;
    public CovidDownloader(string baseUrl, string userName, string password, string connectionString)
    {
        this.baseUrl = baseUrl;
        this.userName = userName;
        this.password = password;
        this.connectionString = connectionString;
    }
    public async Task Execute()
    {
        var tokenUrl = baseUrl + "connect/token";
        var downloadDocUrl = baseUrl + "ayanova/documents/";

        var covidVaccine2 = 29544;
        var sql = @"SELECT top 1000
r.id RequirementId
,r.CandidateDocumentId
,r.CandidateUserInfoId
,d.Id DocumentId
,d.Path
FROM nurses.compliance.requirements r
JOIN nurses.compliance.candidatedocuments cd on r.CandidateDocumentId = cd.Id
JOIN nurses.documents.Documents d on cd.DocumentId = d.Id
WHERE DocumentTypeId = @DocumentTypeId
order by r.id desc";

        using var connection = new SqlConnection(connectionString);
        var parameters = new { DocumentTypeId = covidVaccine2 };
        var query = await connection.QueryAsync<CovidDoc>(sql, parameters);
        var documents = query.ToList();
        var token = await HttpRequestFactory.GetApiToken(userName, password, tokenUrl);
        foreach (var document in documents)
        {
            try
            {
                var request = new HttpRequestBuilder();
                request.AddBearerToken(token);
                request.AddMethod(HttpMethod.Get);
                request.AddRequestUri(downloadDocUrl + document.DocumentId);
                var docResult = await request.SendAsync();
                if (docResult.IsSuccessStatusCode)
                {
                    Directory.CreateDirectory("docs");
                    using var fs = new FileStream($"docs/{document.DocumentId}-{document.CandidateDocumentId}-{document.RequirementId}-{document.CandidateUserInfoId}.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
                    await docResult.Content.CopyToAsync(fs);
                }
                else
                {
                    Console.WriteLine(document.DocumentId + " " + docResult.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(document.DocumentId + " " + ex.ToString());
            }
        }
    }
}
