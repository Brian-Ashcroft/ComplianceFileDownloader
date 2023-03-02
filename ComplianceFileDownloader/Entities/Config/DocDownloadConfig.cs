namespace ComplianceFileDownloader.Entities.Config;

internal record DocDownloadConfig
(
	int DocumentTypeId,
	string DocumentTypeName,
	int PassedDocsToDownload,
	int QMFailedDocsToDownload,
	int CLFailedDocsToDownload
);