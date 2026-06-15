IF COL_LENGTH('dbo.tbl_credit_request', 'signed_pdf_file_name') IS NULL
BEGIN
    ALTER TABLE dbo.tbl_credit_request
        ADD signed_pdf_file_name NVARCHAR(260) NULL,
            signed_pdf_storage_key NVARCHAR(500) NULL;
END
GO
