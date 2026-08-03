IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'tbl_grade_scale_row' AND schema_id = SCHEMA_ID(N'dbo'))
BEGIN
    CREATE TABLE dbo.tbl_grade_scale_row
    (
        id BIGINT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_tbl_grade_scale_row PRIMARY KEY,
        academic_term_key NVARCHAR(64) NOT NULL,
        mark DECIMAL(9, 4) NOT NULL,
        grade DECIMAL(9, 4) NOT NULL,
        sort_order INT NOT NULL CONSTRAINT DF_tbl_grade_scale_row_sort_order DEFAULT (0),
        status BIT NOT NULL CONSTRAINT DF_tbl_grade_scale_row_status DEFAULT (1),
        created_at DATETIME2(7) NULL,
        updated_at DATETIME2(7) NULL
    );

    CREATE INDEX ix_grade_scale_row_academic_term_key
        ON dbo.tbl_grade_scale_row (academic_term_key);
END
GO
