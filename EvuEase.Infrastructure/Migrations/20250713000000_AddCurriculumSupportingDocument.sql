IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.tbl_curricula')
      AND name = N'supporting_document_file_name'
)
BEGIN
    ALTER TABLE dbo.tbl_curricula
    ADD
        supporting_document_file_name NVARCHAR(260) NULL,
        supporting_document_storage_key NVARCHAR(500) NULL,
        supporting_document_uploaded_at DATETIME2 NULL;
END
GO
