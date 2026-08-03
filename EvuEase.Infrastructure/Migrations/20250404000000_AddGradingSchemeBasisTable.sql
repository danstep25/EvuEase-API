IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'tbl_grading_scheme_basis' AND schema_id = SCHEMA_ID(N'dbo'))
BEGIN
    CREATE TABLE dbo.tbl_grading_scheme_basis
    (
        id BIGINT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_tbl_grading_scheme_basis PRIMARY KEY,
        academic_term_key NVARCHAR(64) NOT NULL,
        grading_scheme_code NVARCHAR(32) NOT NULL,
        grading_scheme_description NVARCHAR(200) NOT NULL,
        grading_basis_code NVARCHAR(32) NOT NULL,
        grading_basis_description NVARCHAR(200) NOT NULL,
        status BIT NOT NULL CONSTRAINT DF_tbl_grading_scheme_basis_status DEFAULT (1),
        created_at DATETIME2(7) NULL,
        updated_at DATETIME2(7) NULL
    );

    CREATE UNIQUE INDEX uq_grading_scheme_basis_academic_term_key
        ON dbo.tbl_grading_scheme_basis (academic_term_key);
END
GO
