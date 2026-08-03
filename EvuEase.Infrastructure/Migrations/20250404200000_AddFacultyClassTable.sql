IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'tbl_faculty_class' AND schema_id = SCHEMA_ID(N'dbo'))
BEGIN
    CREATE TABLE dbo.tbl_faculty_class
    (
        id BIGINT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_tbl_faculty_class PRIMARY KEY,
        course_code NVARCHAR(32) NOT NULL,
        class_number NVARCHAR(32) NOT NULL,
        section NVARCHAR(64) NOT NULL,
        course_title NVARCHAR(300) NOT NULL,
        component NVARCHAR(64) NOT NULL,
        academic_term NVARCHAR(200) NOT NULL,
        enrolled_count INT NOT NULL CONSTRAINT DF_tbl_faculty_class_enrolled DEFAULT (0),
        status BIT NOT NULL CONSTRAINT DF_tbl_faculty_class_status DEFAULT (1),
        created_at DATETIME2(7) NULL,
        updated_at DATETIME2(7) NULL
    );

    CREATE INDEX ix_faculty_class_course_code ON dbo.tbl_faculty_class (course_code);
    CREATE INDEX ix_faculty_class_academic_term ON dbo.tbl_faculty_class (academic_term);
END
GO
