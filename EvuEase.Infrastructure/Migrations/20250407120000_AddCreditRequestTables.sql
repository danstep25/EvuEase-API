IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'tbl_credit_request' AND schema_id = SCHEMA_ID(N'dbo'))
BEGIN
    CREATE TABLE dbo.tbl_credit_request
    (
        id BIGINT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_tbl_credit_request PRIMARY KEY,
        credit_request_no NVARCHAR(32) NOT NULL,
        student_id BIGINT NULL,
        student_number NVARCHAR(50) NOT NULL,
        first_name NVARCHAR(100) NOT NULL,
        middle_name NVARCHAR(100) NULL,
        last_name NVARCHAR(100) NOT NULL,
        program_id BIGINT NOT NULL,
        sy_id BIGINT NOT NULL,
        request_status NVARCHAR(32) NOT NULL CONSTRAINT DF_tbl_credit_request_status DEFAULT (N'Pending'),
        status BIT NOT NULL CONSTRAINT DF_tbl_credit_request_active DEFAULT (1),
        created_at DATETIME2(7) NULL,
        updated_at DATETIME2(7) NULL,
        deleted_at DATETIME2(7) NULL,
        deleted_by NVARCHAR(128) NULL
    );

    CREATE UNIQUE INDEX ux_credit_request_no ON dbo.tbl_credit_request (credit_request_no) WHERE deleted_at IS NULL;
    CREATE INDEX ix_credit_request_student_number ON dbo.tbl_credit_request (student_number);
    CREATE INDEX ix_credit_request_program_id ON dbo.tbl_credit_request (program_id);
    CREATE INDEX ix_credit_request_sy_id ON dbo.tbl_credit_request (sy_id);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'tbl_credit_request_line' AND schema_id = SCHEMA_ID(N'dbo'))
BEGIN
    CREATE TABLE dbo.tbl_credit_request_line
    (
        id BIGINT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_tbl_credit_request_line PRIMARY KEY,
        credit_request_id BIGINT NOT NULL,
        sort_order INT NOT NULL,
        applied_course_code NVARCHAR(32) NULL,
        applied_course_title NVARCHAR(200) NULL,
        applied_lec_units DECIMAL(5, 2) NOT NULL CONSTRAINT DF_tbl_credit_request_line_lec DEFAULT (0),
        applied_lab_units DECIMAL(5, 2) NOT NULL CONSTRAINT DF_tbl_credit_request_line_lab DEFAULT (0),
        grade NVARCHAR(16) NULL,
        equivalent_course_code NVARCHAR(32) NULL,
        status BIT NOT NULL CONSTRAINT DF_tbl_credit_request_line_active DEFAULT (1),
        created_at DATETIME2(7) NULL,
        updated_at DATETIME2(7) NULL,
        deleted_at DATETIME2(7) NULL,
        deleted_by NVARCHAR(128) NULL,
        CONSTRAINT FK_tbl_credit_request_line_request
            FOREIGN KEY (credit_request_id) REFERENCES dbo.tbl_credit_request (id) ON DELETE CASCADE
    );

    CREATE INDEX ix_credit_request_line_request_id ON dbo.tbl_credit_request_line (credit_request_id);
END
GO
