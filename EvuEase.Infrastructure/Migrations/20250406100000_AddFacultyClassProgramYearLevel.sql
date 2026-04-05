IF COL_LENGTH('dbo.tbl_faculty_class', 'program_code') IS NULL
BEGIN
    ALTER TABLE dbo.tbl_faculty_class ADD program_code NVARCHAR(32) NOT NULL
        CONSTRAINT DF_tbl_faculty_class_program_code DEFAULT (N'');
END
GO

IF COL_LENGTH('dbo.tbl_faculty_class', 'year_level') IS NULL
BEGIN
    ALTER TABLE dbo.tbl_faculty_class ADD year_level NVARCHAR(64) NOT NULL
        CONSTRAINT DF_tbl_faculty_class_year_level DEFAULT (N'');
END
GO
