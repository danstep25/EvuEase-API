-- Adds temporary-password support for the student portal:
--   * portal_password_must_change: forces a password change on next login
--   * portal_password_expires_at:  expiry of an admin-issued temporary password

IF COL_LENGTH('tbl_students', 'portal_password_must_change') IS NULL
BEGIN
    ALTER TABLE tbl_students
        ADD portal_password_must_change BIT NOT NULL
        CONSTRAINT DF_students_portal_pw_must_change DEFAULT (0);
    PRINT 'Added tbl_students.portal_password_must_change';
END
GO

IF COL_LENGTH('tbl_students', 'portal_password_expires_at') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD portal_password_expires_at DATETIME2 NULL;
    PRINT 'Added tbl_students.portal_password_expires_at';
END
GO

-- Store the admin-issued temporary password on the reset request so the admin
-- can keep viewing it until the student changes it. Cleared on resolution.
IF COL_LENGTH('tbl_student_portal_password_reset_requests', 'temporary_password') IS NULL
BEGIN
    ALTER TABLE tbl_student_portal_password_reset_requests ADD temporary_password NVARCHAR(255) NULL;
    PRINT 'Added tbl_student_portal_password_reset_requests.temporary_password';
END
GO

IF COL_LENGTH('tbl_student_portal_password_reset_requests', 'temporary_password_expires_at') IS NULL
BEGIN
    ALTER TABLE tbl_student_portal_password_reset_requests ADD temporary_password_expires_at DATETIME2 NULL;
    PRINT 'Added tbl_student_portal_password_reset_requests.temporary_password_expires_at';
END
GO
