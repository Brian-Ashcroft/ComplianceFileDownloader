namespace ComplianceFileDownloader.Entities
{
    internal class ExampleDoc
    {
        public int DocumentTypeId { get; init; }
        public string DocumentTypeName { get; init; }
        public string CategoryName { get; init; }
        public string LOB { get; init; }
        public int UsedInLast12Months { get; init; }
        public int DocumentId { get; init; }
        public string TaskType { get; set; }
    }
}
