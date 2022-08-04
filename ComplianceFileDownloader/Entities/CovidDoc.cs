namespace ComplianceFileDownloader.Entities;

internal record struct CovidDoc
{
    public int RequirementId { get; init; }
    public int CandidateDocumentId { get; init; }
    public int CandidateUserInfoId { get; init; }
    public int DocumentId { get; init; }
    public string Path { get; init; }
    public int DocumentTypeId { get; init; }
}


