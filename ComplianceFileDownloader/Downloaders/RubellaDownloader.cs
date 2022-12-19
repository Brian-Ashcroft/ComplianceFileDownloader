using ComplianceFileDownloader.Entities;
using Dapper;
using System.Data.SqlClient;
using System.Text;

namespace ComplianceFileDownloader.Downloaders
{
    internal class RubellaDownloader
    {
        private string baseUrl;
        private string userName;
        private string password;
        private string connectionString;
        public RubellaDownloader(string baseUrl, string userName, string password, string connectionString)
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

            var rubellaId = 32;

            int fileCount;
            var queries = new List<queryDto>();
            queries.Add(new queryDto(750,
                        @"SELECT DISTINCT TOP 2000
	                            r.CandidateDocumentId
                               ,r.DocumentTypeId
                               ,d.Id DocumentId
                               ,d.Path
                               ,'Approved' AS Status
                               ,'' AS Reason
                               ,u.FirstName
                               ,u.LastName
                               ,r.ExpirationDate
                               ,r.Id
                               ,fdtd.[Description] AS 'FacilityDescription'
                               ,adtd.[Description] AS 'AssociationDescription'

                            FROM nurses.Compliance.Requirements r
                            JOIN nurses.Compliance.CandidateDocuments cd
	                            ON r.CandidateDocumentId = cd.Id
                            JOIN nurses.Documents.Documents d
	                            ON cd.DocumentId = d.Id
                            JOIN nurses.dbo.UserInfo u
	                            ON u.userID = cd.CandidateUserInfoId
                            LEFT JOIN [nurses].dbo.ContractInfo ci
	                            ON r.ContractId = ci.ContractId
                            LEFT JOIN nurses.Compliance.DocumentTypeConfigurations fdtc
	                            ON fdtc.DocumentTypeId = @DocumentTypeId
		                            AND ci.hospID = fdtc.FacilityId
		                            AND fdtc.DocumentTypeDescriptionTypesId = 0 --internaldesc = 0
                            LEFT JOIN nurses.Compliance.DocumentTypeDescriptions fdtd
	                            ON fdtd.DocumentTypeDescriptionId = fdtc.DocumentTypeDescriptionId
                            LEFT JOIN [nurses].dbo.FacilityProfiles fp
	                            ON fp.Id = ci.hospID
                            LEFT JOIN nurses.Compliance.DocumentTypeConfigurations adtc
	                            ON adtc.DocumentTypeId = @DocumentTypeId
		                            AND fp.QMSystemCode = adtc.QmAssoicationId
		                            AND adtc.DocumentTypeDescriptionTypesId = 0 --internaldesc = 0
                            LEFT JOIN nurses.Compliance.DocumentTypeDescriptions adtd
	                            ON adtd.DocumentTypeDescriptionId = adtc.DocumentTypeDescriptionId

                            WHERE r.DocumentTypeId = @DocumentTypeId
                            AND ValidationStatusId = 2
                            ORDER BY r.Id DESC"));

            queries.Add(new queryDto(125,
                        @"SELECT DISTINCT TOP 500
	                            r.CandidateDocumentId
                               ,r.DocumentTypeId
                               ,d.Id DocumentId
                               ,d.Path
                               ,'Declined' AS Status
                               ,rdn.Note AS Reason
                               ,u.FirstName
                               ,u.LastName
                               ,r.ExpirationDate
                               ,rdn.Id
                               ,fdtd.[Description] AS 'FacilityDescription'
                               ,adtd.[Description] AS 'AssociationDescription'
                            FROM [nurses].[Compliance].[RequirementDeclinedNotes] rdn
                            JOIN [nurses].[Compliance].[Requirements] r
	                            ON r.Id = rdn.RequirementId
		                            AND rdn.CandidateDocumentId = r.CandidateDocumentId
                            JOIN [nurses].[Compliance].[CandidateDocuments] cd
	                            ON cd.Id = r.CandidateDocumentId
                            JOIN [nurses].[Documents].[Documents] d
	                            ON d.Id = cd.DocumentId
                            JOIN [nurses].[dbo].[UserInfo] u
	                            ON u.UserID = cd.CandidateUserInfoId
                            LEFT JOIN [nurses].dbo.ContractInfo ci
	                            ON r.contractid = ci.contractid
                            LEFT JOIN nurses.Compliance.DocumentTypeConfigurations fdtc
	                            ON fdtc.DocumentTypeId = @DocumentTypeId
		                            AND ci.hospID = fdtc.FacilityId
		                            AND fdtc.DocumentTypeDescriptionTypesId = 0 --internaldesc = 0
                            LEFT JOIN nurses.Compliance.DocumentTypeDescriptions fdtd
	                            ON fdtd.DocumentTypeDescriptionId = fdtc.DocumentTypeDescriptionId
                            LEFT JOIN [nurses].dbo.FacilityProfiles fp
	                            ON fp.Id = ci.hospID
                            LEFT JOIN nurses.Compliance.DocumentTypeConfigurations adtc
	                            ON adtc.DocumentTypeId = @DocumentTypeId
		                            AND fp.QMSystemCode = adtc.QmAssoicationId
		                            AND adtc.DocumentTypeDescriptionTypesId = 0 --internaldesc = 0
                            LEFT JOIN nurses.Compliance.DocumentTypeDescriptions adtd
	                            ON adtd.DocumentTypeDescriptionId = adtc.DocumentTypeDescriptionId
                            WHERE r.DocumentTypeId = @DocumentTypeId
                            ORDER BY rdn.Id DESC"));

            queries.Add(new queryDto(125,
                        @"SELECT DISTINCT TOP 500
	                            cd.Id CandidateDocumentId
                               ,crp.DocumentTypeId
                               ,d.Id DocumentId
                               ,d.Path
                               ,'Rejected' AS Status
                               ,cdn.Note AS Reason
                               ,u.FirstName
                               ,u.LastName
                               ,r.ExpirationDate
                               ,cdn.Id
                               ,fdtd.[Description] AS 'FacilityDescription'
                               ,adtd.[Description] AS 'AssociationDescription'
                            FROM [nurses].[Compliance].[CandidateDocumentNotes] cdn
                            JOIN (SELECT DISTINCT
		                            CandidateDocumentId
	                               ,DocumentTypeId
	                            FROM nurses.Compliance.CandidateDocumentRejectedPages) crp
	                            ON cdn.CandidateDocumentId = crp.CandidateDocumentId
                            JOIN [nurses].[Compliance].[CandidateDocuments] cd
	                            ON cd.Id = cdn.CandidateDocumentId
                            JOIN [nurses].[Documents].[Documents] d
	                            ON d.Id = cd.DocumentId
                            JOIN [nurses].[dbo].[UserInfo] u
	                            ON u.UserID = cd.CandidateUserInfoId
                            LEFT JOIN [nurses].[Compliance].[Requirements] r
	                            ON r.CandidateDocumentId = cd.Id
                            LEFT JOIN nurses.dbo.ContractInfo ci
	                            ON r.contractid = ci.contractid
                            LEFT JOIN nurses.Compliance.DocumentTypeConfigurations fdtc
	                            ON fdtc.DocumentTypeId = @DocumentTypeId
		                            AND ci.hospID = fdtc.FacilityId
		                            AND fdtc.DocumentTypeDescriptionTypesId = 0 --internaldesc = 0
                            LEFT JOIN nurses.Compliance.DocumentTypeDescriptions fdtd
	                            ON fdtd.DocumentTypeDescriptionId = fdtc.DocumentTypeDescriptionId
                            LEFT JOIN [nurses].dbo.FacilityProfiles fp
	                            ON fp.Id = ci.hospID
                            LEFT JOIN nurses.Compliance.DocumentTypeConfigurations adtc
	                            ON adtc.DocumentTypeId = @DocumentTypeId
		                            AND fp.QmSystemCode = adtc.QmAssoicationId
		                            AND adtc.DocumentTypeDescriptionTypesId = 0 --internaldesc = 0
                            LEFT JOIN nurses.Compliance.DocumentTypeDescriptions adtd
	                            ON adtd.DocumentTypeDescriptionId = adtc.DocumentTypeDescriptionId
                            WHERE crp.DocumentTypeId = @DocumentTypeId
                            AND cdn.TypeId = 1
                            ORDER BY cdn.Id DESC"));

            using var connection = new SqlConnection(connectionString);
            var parameters = new { DocumentTypeId = rubellaId };
            foreach (var query in queries)
            {
                var result = await connection.QueryAsync<RubellaDoc>(query.Sql, parameters);
                var documents = result.ToList();
                var token = await HttpRequestFactory.GetApiToken(userName, password, tokenUrl);
                fileCount = 0;

                foreach (var document in documents)
                {
                    if(fileCount >= query.Count) break;

                    if (File.Exists($"rubella_docs/{document.DocumentId}.pdf"))
                    {
                        csv.AppendLine($"{document.DocumentTypeId}," +
                            $" {document.CandidateDocumentId}," +
                            $" {document.DocumentId}," +
                            $" {document.Status}," +
                            $" {Sanitze(document.Reason)}," +
                            $" {document.FirstName}," +
                            $" {document.LastName}," +
                            $" {document.ExpirationDate}," +
                            $" {Sanitze(document.FacilityDescription)}," +
                            $" {Sanitze(document.AssociationDescription)}, DUP");
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
                            csv.AppendLine($"{document.DocumentTypeId}," +
                                $" {document.CandidateDocumentId}," +
                                $" {document.DocumentId}," +
                                $" {document.Status}," +
                                $" {Sanitze(document.Reason)}," +
                                $" {document.FirstName}," +
                                $" {document.LastName}," +
                                $" {document.ExpirationDate}," +
                                $" {Sanitze(document.FacilityDescription)}," +
                                $" {Sanitze(document.AssociationDescription)}");
                            Directory.CreateDirectory("rubella_docs");
                            using var fs = new FileStream($"rubella_docs/{document.DocumentId}.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
                            await docResult.Content.CopyToAsync(fs);
                            fileCount++;
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
            File.WriteAllText("rubella_docs.csv", csv.ToString());
        }

        public string Sanitze(string s)
        {
            if (s == null) return s;

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