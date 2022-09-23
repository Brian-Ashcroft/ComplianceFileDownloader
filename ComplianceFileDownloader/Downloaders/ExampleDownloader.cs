using ComplianceFileDownloader.Entities;
using Dapper;
using System.Data.SqlClient;
using System.Text;

namespace ComplianceFileDownloader.Downloaders;
internal class ExampleDownloader
{
    private string baseUrl;
    private string userName;
    private string password;
    private string connectionString;
    public ExampleDownloader(string baseUrl, string userName, string password, string connectionString)
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

        var sql = @"select
dt.id as DocumentTypeId
,dt.Name as DocumentTypeName
,dtc.Name as CategoryName
,lob.name as LOB
,c.total as UsedInLast12Months
,ctt.name as TaskType
,document.documentId
from nurses.compliance.documenttypes dt
left join nurses.compliance.candidateTaskTypes ctt on ctt.id = dt.candidatetasktypeid
join nurses.compliance.documenttypecategories dtc on dt.documenttypecategoryid = dtc.id
join nurses.dbo.linesofbusiness lob on dtc.LineOfBusinessId = lob.id
left join nurses.dbo.facilityprofiles fp on dt.hospid = fp.id
left join nurses.facility.contractgroups cg on dt.associationid = cg.id
outer apply (
	select count(*) as total from nurses.compliance.requirements r
	where  r.documenttypeid = dt.id
	and r.contractId is not null
	and r.candidatedocumentid is not null
	and (r.ValidationStatusId is null or r.ValidationStatusId = 2)
	and r.documentassigneddate > DATEADD(month, -12, GETDATE())
	group by documenttypeid) as c
outer apply (
	select top 1 documentId from nurses.compliance.requirements r
	join nurses.compliance.candidatedocuments cd on r.candidatedocumentid = cd.id
	where  r.documenttypeid = dt.id
	and r.contractId is not null
	and (r.ValidationStatusId is null or r.ValidationStatusId = 2)
	order by r.id desc) as document
where dt.visibility = 1 
and dt.isdeleted = 0
and dt.name not like '%DO NOT REQUIRE%'
and dt.name not like '%DO NOT USE%'
and dt.name not like '%trackrncompliance%' 
and dt.name not like '%docmaster%'
and len(dt.name) > 1
and (fp.id is null or (fp.isdeleted = 0
and fp.name not like 'DO NOT USE%'
and fp.name not like 'DUPLICATE%'
and fp.name not like 'CLOSED%'))
and (cg.id is null or (
cg.name not like 'DO NOT USE%'
and cg.name not like 'DUPLICATE%'))
and c.total >= 100 
and dt.CandidateTaskTypeId <> 3 and dt.CandidateTaskTypeId <> 8";

        using var connection = new SqlConnection(connectionString);
        var query = await connection.QueryAsync<ExampleDoc>(sql);
        var documents = query.ToList();
        var token = await HttpRequestFactory.GetApiToken(userName, password, tokenUrl);
        var csv = new StringBuilder();
        csv.AppendLine("DocumentTypeId, DocumentTypeName, CategoryName, LOB, UsedInLast12Months, TaskType, DocumentId");
        foreach (var document in documents)
        {
            if (File.Exists($"exampleDocs/{document.DocumentTypeId}.pdf"))
            {
                Directory.CreateDirectory("exampleDocs/Docsfiltered");
                File.Copy($"exampleDocs/{document.DocumentTypeId}.pdf", $"exampleDocs/Docsfiltered/{document.DocumentTypeId}.pdf");
                csv.AppendLine($"{document.DocumentTypeId}, {document.DocumentTypeName.Replace(",", "")}, {document.CategoryName}, {document.LOB}, {document.UsedInLast12Months}, {document.TaskType}, {document.DocumentId}");
                continue;
            }
            try
            {
                Console.WriteLine(document.DocumentTypeName);
                //var request = new HttpRequestBuilder();
                //request.AddBearerToken(token);
                //request.AddMethod(HttpMethod.Get);
                //request.AddRequestUri(downloadDocUrl + document.DocumentId);
                //var docResult = await request.SendAsync();
                //if (docResult.IsSuccessStatusCode)
                //{
                //    csv.AppendLine($"{document.DocumentTypeId}, {document.DocumentTypeName.Replace(",", "")}, {document.CategoryName}, {document.LOB}, {document.UsedInLast12Months}, {document.TaskType},  {document.DocumentId}");
                //    Directory.CreateDirectory("exampleDocs");
                //    using var fs = new FileStream($"exampleDocs/{document.DocumentTypeId}.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
                //    await docResult.Content.CopyToAsync(fs);
                //}
                //else
                //{
                //    Console.WriteLine(document.DocumentId + " " + docResult.ReasonPhrase);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(document.DocumentId + " " + ex.ToString());
            }
        }
        File.WriteAllText("ExampleDocs-filtered.csv", csv.ToString());
    }
}
