namespace ComplianceFileDownloader.Entities;

internal record struct DriversLicenseDoc
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
    public string FacilityDescription { get; set; }
    public string AssociationDescription { get; set; }
    public string StateCode { get; set; }
    public string State { get; set; }

}

