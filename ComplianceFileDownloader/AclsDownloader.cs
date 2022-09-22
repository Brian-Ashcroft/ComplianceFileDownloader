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
            csv.AppendLine("DocumentTypeId, CandidateDocumentId, DocumentId, Status, Reason, FirstName, LastName, ExpirationDate, FacilityDescription, AssociationDescription");

            var aclsId = 13;
            var queries = new List<string>();
            queries.Add(@"SELECT distinct top 10
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
		                    ,hdesc.comments as 'FacilityDescription'
		                    ,adesc.comments as 'AssociationDescription'
                            FROM nurses.compliance.requirements r
                            JOIN nurses.compliance.candidatedocuments cd on r.CandidateDocumentId = cd.Id
                            JOIN nurses.documents.Documents d on cd.DocumentId = d.Id
                            JOIN nurses.dbo.UserInfo u on u.userid = cd.candidateuserinfoid
		                    LEFT JOIN [nurses].dbo.contractinfo ci on r.contractid = ci.contractid
		                    LEFT JOIN [nurses].dbo.hospQMrequire hdesc on ci.hospid = hdesc.hospid AND hdesc.docid = @DocumentTypeId
		                    LEFT JOIN [nurses].dbo.facilityprofiles fp on fp.id = ci.hospid
		                    LEFT JOIN [nurses].dbo.hospQMrequire adesc on adesc.associationid = fp.qmsystemcode AND adesc.docid = @DocumentTypeId
                            WHERE DocumentTypeId = @DocumentTypeId
                            AND ValidationStatusId = 2
                            order by r.id desc");

            queries.Add(@"SELECT distinct TOP 5 
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
		                    ,hdesc.comments as 'FacilityDescription'
		                    ,adesc.comments as 'AssociationDescription'
                            FROM [nurses].[Compliance].[RequirementDeclinedNotes] rdn
                            JOIN [nurses].[Compliance].[Requirements] r ON r.Id = rdn.RequirementId and rdn.candidatedocumentid = r.candidatedocumentid
                            JOIN [nurses].[Compliance].[CandidateDocuments] cd ON cd.Id = r.CandidateDocumentId
                            JOIN [nurses].[Documents].[Documents] d ON d.Id = cd.DocumentId
                            JOIN [nurses].[dbo].[UserInfo] u on u.userid = cd.candidateuserinfoid
		                    LEFT JOIN [nurses].dbo.contractinfo ci on r.contractid = ci.contractid
		                    LEFT JOIN [nurses].dbo.hospQMrequire hdesc on ci.hospid = hdesc.hospid AND hdesc.docid = @DocumentTypeId
		                    LEFT JOIN [nurses].dbo.facilityprofiles fp on fp.id = ci.hospid
		                    LEFT JOIN [nurses].dbo.hospQMrequire adesc on adesc.associationid = fp.qmsystemcode AND adesc.docid = @DocumentTypeId
                            WHERE r.DocumentTypeId = @DocumentTypeId
                            ORDER BY rdn.Id desc");

            queries.Add(@"SELECT distinct TOP 5 
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
			                ,hdesc.comments as 'FacilityDescription'
			                ,adesc.comments as 'AssociationDescription'
                            FROM [nurses].[Compliance].[CandidateDocumentNotes] cdn
                            JOIN (SELECT distinct CandidateDocumentId, DocumentTypeId FROM nurses.compliance.candidatedocumentrejectedpages) crp ON cdn.CandidateDocumentId = crp.CandidateDocumentId
                            JOIN [nurses].[Compliance].[CandidateDocuments] cd ON cd.Id = cdn.CandidateDocumentId
                            JOIN [nurses].[Documents].[Documents] d ON d.Id = cd.DocumentId
                            JOIN [nurses].[dbo].[UserInfo] u on u.userid = cd.candidateuserinfoid
                            LEFT JOIN [nurses].[compliance].[requirements] r on r.CandidateDocumentId = cd.Id
			                LEFT JOIN nurses.dbo.contractinfo ci on r.contractid = ci.contractid
			                LEFT JOIN nurses.dbo.hospQMrequire hdesc on ci.hospid = hdesc.hospid AND hdesc.docid = @DocumentTypeId
			                LEFT JOIN nurses.dbo.facilityprofiles fp on fp.id = ci.hospid
			                LEFT JOIN nurses.dbo.hospQMrequire adesc on adesc.associationid = fp.qmsystemcode AND adesc.docid = @DocumentTypeId
                            WHERE crp.DocumentTypeId = @DocumentTypeId
                            and cdn.TypeId = 1
                            ORDER BY cdn.Id desc");

            using var connection = new SqlConnection(connectionString);
            var parameters = new { DocumentTypeId = aclsId };
            foreach (var sql in queries)
            {
                var query = await connection.QueryAsync<AclsDoc>(sql, parameters);
                var documents = query.ToList();
                var token = await HttpRequestFactory.GetApiToken(userName, password, tokenUrl);
                foreach (var document in documents)
                {
                    if (File.Exists($"acls_docs/{document.DocumentId}.pdf"))
                    {
                        csv.AppendLine($"{document.DocumentTypeId}, {document.CandidateDocumentId}, {document.DocumentId}, {document.Status}, {Sanitze(document.Reason)}, {document.FirstName}, {document.LastName}, {document.ExpirationDate}, {Sanitze(document.FacilityDescription)}, {Sanitze(document.AssociationDescription)}, DUP");
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
                            csv.AppendLine($"{document.DocumentTypeId}, {document.CandidateDocumentId}, {document.DocumentId}, {document.Status}, {Sanitze(document.Reason)}, {document.FirstName}, {document.LastName}, {document.ExpirationDate}, {Sanitze(document.FacilityDescription)}, {Sanitze(document.AssociationDescription)}");
                            Directory.CreateDirectory("acls_docs");
                            using var fs = new FileStream($"acls_docs/{document.DocumentId}.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
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
            File.WriteAllText("acls_docs.csv", csv.ToString());
        }

        public string Sanitze(string s)
        {
            if(s == null) return s;

            s = s.Replace(",", " ");
            s = s.Replace("\r", " ");
            s = s.Replace("\n", " ");
            s = s.ToLower().Replace("<li>", " ");
            s = s.ToLower().Replace("<br>", " ");
            s = s.ToLower().Replace("<b>", "");
            s = s.ToLower().Replace("</b>", "");

            return s;
        }
    }
}