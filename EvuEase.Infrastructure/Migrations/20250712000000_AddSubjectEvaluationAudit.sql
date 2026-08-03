IF OBJECT_ID('tbl_subject_evaluation_audit', 'U') IS NULL
BEGIN
    CREATE TABLE tbl_subject_evaluation_audit
    (
        id                  BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        student_id          BIGINT NOT NULL,
        student_number      NVARCHAR(50) NOT NULL,
        student_name        NVARCHAR(255) NOT NULL,
        program_code        NVARCHAR(50) NOT NULL,
        program_year_level  NVARCHAR(100) NOT NULL,
        school_year         NVARCHAR(50) NOT NULL,
        semester            NVARCHAR(50) NOT NULL,
        school_year_term    NVARCHAR(200) NOT NULL,
        total_units_selected INT NOT NULL,
        evaluated_by        NVARCHAR(128) NOT NULL,
        evaluated_at        DATETIME2 NOT NULL,
        evaluation_payload  NVARCHAR(MAX) NOT NULL,
        status              BIT NOT NULL CONSTRAINT DF_subject_eval_audit_status DEFAULT (1),
        created_at          DATETIME2 NULL,
        updated_at          DATETIME2 NULL,
        deleted_at          DATETIME2 NULL,
        deleted_by          NVARCHAR(128) NULL
    );

    CREATE INDEX ix_subject_evaluation_audit_evaluated_at
        ON tbl_subject_evaluation_audit (evaluated_at DESC);

    CREATE INDEX ix_subject_evaluation_audit_student_id
        ON tbl_subject_evaluation_audit (student_id);

    PRINT 'Created tbl_subject_evaluation_audit';
END
GO
