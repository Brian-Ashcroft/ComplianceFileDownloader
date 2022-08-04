using ComplianceFileDownloader.Entities;
using Dapper;
using System.Data.SqlClient;
using System.Text;

namespace ComplianceFileDownloader
{
    internal class AclsDownloader
    {
        private string baseUrl;
        private string userName;
        private string password;
        private string connectionString;
        public AclsDownloader(string baseUrl, string userName, string password, string connectionString)
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

            var csv = new StringBuilder();
            csv.AppendLine("DocumentTypeId, CandidateDocumentId, DocumentId, Status, Reason, FirstName, LastName, ExpirationDate");

            var blsId = 14;
            var queries = new List<string>();
            queries.Add(@"SELECT distinct top 1000
                            r.CandidateDocumentId
                            ,DocumentTypeId
                            ,d.Id DocumentId
                            ,d.Path
                            ,'Approved' as Status
                            ,'' as Reason
                            ,u.FirstName
                            ,u.LastName
                            ,r.ExpirationDate
                            ,r.id
                            FROM nurses.compliance.requirements r
                            JOIN nurses.compliance.candidatedocuments cd on r.CandidateDocumentId = cd.Id
                            JOIN nurses.documents.Documents d on cd.DocumentId = d.Id
                            JOIN nurses.dbo.UserInfo u on u.userid = cd.candidateuserinfoid
                            WHERE DocumentTypeId = @DocumentTypeId
                            AND ValidationStatusId = 2
                            order by r.id desc");

            queries.Add(@"SELECT distinct TOP 25 
                            r.CandidateDocumentId
                            ,r.DocumentTypeId
                            ,d.Id DocumentId
                            ,d.Path
                            ,'Declined' As Status
                            ,rdn.Note as Reason
                            ,u.FirstName
                            ,u.LastName
                            ,r.ExpirationDate
                            ,rdn.id
                            FROM [nurses].[Compliance].[RequirementDeclinedNotes] rdn
                            JOIN [nurses].[Compliance].[Requirements] r ON r.Id = rdn.RequirementId and rdn.candidatedocumentid = r.candidatedocumentid
                            JOIN [nurses].[Compliance].[CandidateDocuments] cd ON cd.Id = r.CandidateDocumentId
                            JOIN [nurses].[Documents].[Documents] d ON d.Id = cd.DocumentId
                            JOIN [nurses].[dbo].[UserInfo] u on u.userid = cd.candidateuserinfoid
                            WHERE r.DocumentTypeId = @documentTypeId
                            ORDER BY rdn.Id desc");

            queries.Add(@"SELECT distinct TOP 25 
                            cd.Id CandidateDocumentId
                            ,crp.DocumentTypeId
                            ,d.Id DocumentId
                            ,d.Path
                            ,'Rejected' As Status
                            ,cdn.Note as Reason
                            ,u.FirstName
                            ,u.LastName
                            ,r.ExpirationDate
                            ,cdn.id
                            FROM [nurses].[Compliance].[CandidateDocumentNotes] cdn
                            JOIN (SELECT distinct CandidateDocumentId, DocumentTypeId FROM nurses.compliance.candidatedocumentrejectedpages) crp ON cdn.CandidateDocumentId = crp.CandidateDocumentId
                            JOIN [nurses].[Compliance].[CandidateDocuments] cd ON cd.Id = cdn.CandidateDocumentId
                            JOIN [nurses].[Documents].[Documents] d ON d.Id = cd.DocumentId
                            JOIN [nurses].[dbo].[UserInfo] u on u.userid = cd.candidateuserinfoid
                            LEFT JOIN [nurses].[compliance].[requirements] r on r.CandidateDocumentId = cd.Id
                            WHERE crp.DocumentTypeId = @documentTypeId
                            and cdn.TypeId = 1
                            ORDER BY cdn.Id desc");

            using var connection = new SqlConnection(connectionString);
            var parameters = new { DocumentTypeId = blsId };
            foreach (var sql in queries)
            {
                var query = await connection.QueryAsync<BlsDoc>(sql, parameters);
                var documents = query.ToList();
                var token = await HttpRequestFactory.GetApiToken(userName, password, tokenUrl);
                foreach (var document in documents)
                {
                    if (File.Exists($"bls_docs/{document.DocumentId}.pdf"))
                    {
                        csv.AppendLine($"{document.DocumentTypeId}, {document.CandidateDocumentId}, {document.DocumentId}, {document.Status}, {Sanitze(document.Reason)}, {document.FirstName}, {document.LastName}, {document.ExpirationDate}");
                        continue;
                    }
                    try
                    {
                        var request = new HttpRequestBuilder();
                        request.AddBearerToken(token);
                        request.AddMethod(HttpMethod.Get);
                        request.AddRequestUri(downloadDocUrl + document.DocumentId);
                        var docResult = await request.SendAsync();
                        if (docResult.IsSuccessStatusCode)
                        {
                            csv.AppendLine($"{document.DocumentTypeId}, {document.CandidateDocumentId}, {document.DocumentId}, {document.Status}, {Sanitze(document.Reason)}, {document.FirstName}, {document.LastName}, {document.ExpirationDate}");
                            Directory.CreateDirectory("bls_docs");
                            using var fs = new FileStream($"bls_docs/{document.DocumentId}.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
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
            File.WriteAllText("bls_docs.csv", csv.ToString());
        }

        public string Sanitze(string s)
        {
            s = s.Replace(",", " ");
            s = s.Replace("\r", " ");
            s = s.Replace("\n", " ");

            return s;
        }
    }
}