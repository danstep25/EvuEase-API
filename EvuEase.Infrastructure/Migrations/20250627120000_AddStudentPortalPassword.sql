-- Student portal password (optional, set by registrar) and password reset requests.

IF COL_LENGTH('tbl_students', 'portal_password_hash') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD portal_password_hash NVARCHAR(255) NULL;
    PRINT 'Added tbl_students.portal_password_hash';
END
GO

IF OBJECT_ID('tbl_student_portal_password_reset_requests', 'U') IS NULL
BEGIN
    CREATE TABLE tbl_student_portal_password_reset_requests (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        student_id BIGINT NOT NULL,
        student_number NVARCHAR(50) NOT NULL,
        reason NVARCHAR(500) NULL,
        status NVARCHAR(30) NOT NULL CONSTRAINT DF_sppr_status DEFAULT (N'Pending'),
        registrar_notes NVARCHAR(500) NULL,
        resolved_by NVARCHAR(200) NULL,
        requested_at DATETIME2 NOT NULL CONSTRAINT DF_sppr_requested_at DEFAULT (SYSUTCDATETIME()),
        resolved_at DATETIME2 NULL,
        CONSTRAINT FK_sppr_student FOREIGN KEY (student_id) REFERENCES tbl_students(id)
    );

    CREATE INDEX IX_sppr_status ON tbl_student_portal_password_reset_requests(status);
    CREATE INDEX IX_sppr_student_id ON tbl_student_portal_password_reset_requests(student_id);

    PRINT 'Created tbl_student_portal_password_reset_requests';
END
GO
