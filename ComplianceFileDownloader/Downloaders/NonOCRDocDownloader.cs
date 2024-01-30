using ComplianceFileDownloader.Entities;
using ComplianceFileDownloader.Entities.Config;
using Dapper;
using System.Data.SqlClient;

namespace ComplianceFileDownloader.Downloaders
{
    internal class NonOCRDocDownloader
    {
		private readonly BlobStorageConfig _blobConfig;
		private readonly int _docTypeId;
        public NonOCRDocDownloader(BlobStorageConfig blobConfig, int documentTypeId)
        {
            _blobConfig = blobConfig;
            _docTypeId = documentTypeId;
        }
        public async Task Execute()
        {
            var tokenUrl = _blobConfig.BaseUrl + "connect/token";
            var downloadDocUrl = _blobConfig.BaseUrl + "ayanova/documents/";

            int fileCount;
            var queries = new List<queryDto>
            {
                new queryDto(30,
                    @"SELECT DISTINCT TOP 200 r.CandidateDocumentId
                        ,r.DocumentTypeId
                        ,d.Id DocumentId
                        ,d.Path
                    FROM nurses.Compliance.Requirements r
                    JOIN nurses.Compliance.CandidateDocuments cd
	                    ON r.CandidateDocumentId = cd.Id
                    JOIN nurses.Documents.Documents d
	                    ON cd.DocumentId = d.Id
                    WHERE r.DocumentTypeId = @DocumentTypeId"),
            };

            using var connection = new SqlConnection(_blobConfig.ConnectionString);
            var parameters = new { DocumentTypeId = _docTypeId };
            foreach (var query in queries)
            {
                var result = await connection.QueryAsync<Doc>(query.Sql, parameters);
                var documents = result.ToList();
                var token = await HttpRequestFactory.GetApiToken(_blobConfig.UserName, _blobConfig.Password, tokenUrl);
                fileCount = 0;

                foreach (var document in documents)
                {
                    if(fileCount >= query.Count) break;

                    if (File.Exists($"non_ocr_docs/DocType-{_docTypeId}-{document.DocumentId}.pdf"))
                    {
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
                           
                            Directory.CreateDirectory($"non_ocr_docs");
                            using var fs = new FileStream($"non_ocr_docs/DocType-{_docTypeId}-{document.DocumentId}.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
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
                        continue;
                    }
                }
            }
        }
    }
}