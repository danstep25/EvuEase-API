-- Profile / contact columns for tbl_students (address, contact, email, gender, birthdate).

IF COL_LENGTH('tbl_students', 'address') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD address NVARCHAR(500) NULL;
END
GO

IF COL_LENGTH('tbl_students', 'contact_number') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD contact_number NVARCHAR(50) NULL;
END
GO

IF COL_LENGTH('tbl_students', 'email') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD email NVARCHAR(200) NULL;
END
GO

IF COL_LENGTH('tbl_students', 'gender') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD gender NVARCHAR(20) NULL;
END
GO

IF COL_LENGTH('tbl_students', 'birthdate') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD birthdate DATE NULL;
END
GO
