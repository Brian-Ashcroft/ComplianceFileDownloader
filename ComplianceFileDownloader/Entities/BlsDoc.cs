namespace ComplianceFileDownloader.Entities;

internal record struct BlsDoc
{
    public int DocumentTypeId { get; set; }
    public int? CandidateDocumentId { get; init; }
    public int DocumentId { get; init; }
    public string Path { get; init; }
    public string Reason { get; init; }
    public string Status { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime? ExpirationDate { get; init; }
}



