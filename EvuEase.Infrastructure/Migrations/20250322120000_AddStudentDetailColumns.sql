-- Adds student profile columns to tbl_students when missing (aligns with Student entity / StudentConfiguration).

IF COL_LENGTH('tbl_students', 'student_number') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD student_number NVARCHAR(50) NOT NULL CONSTRAINT DF_tbl_students_student_number DEFAULT '';
END
GO

IF COL_LENGTH('tbl_students', 'first_name') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD first_name NVARCHAR(100) NOT NULL CONSTRAINT DF_tbl_students_first_name DEFAULT '';
END
GO

IF COL_LENGTH('tbl_students', 'last_name') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD last_name NVARCHAR(100) NOT NULL CONSTRAINT DF_tbl_students_last_name DEFAULT '';
END
GO

IF COL_LENGTH('tbl_students', 'middle_name') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD middle_name NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('tbl_students', 'program_code') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD program_code NVARCHAR(50) NOT NULL CONSTRAINT DF_tbl_students_program_code DEFAULT '';
END
GO

IF COL_LENGTH('tbl_students', 'program_title') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD program_title NVARCHAR(300) NOT NULL CONSTRAINT DF_tbl_students_program_title DEFAULT '';
END
GO

IF COL_LENGTH('tbl_students', 'year_level') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD year_level NVARCHAR(50) NOT NULL CONSTRAINT DF_tbl_students_year_level DEFAULT '';
END
GO

IF COL_LENGTH('tbl_students', 'student_type') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD student_type NVARCHAR(50) NOT NULL CONSTRAINT DF_tbl_students_student_type DEFAULT 'Regular';
END
GO

IF COL_LENGTH('tbl_students', 'enrollment_status') IS NULL
BEGIN
    ALTER TABLE tbl_students ADD enrollment_status NVARCHAR(50) NOT NULL CONSTRAINT DF_tbl_students_enrollment_status DEFAULT 'Active';
END
GO
