-- Per-enrollment official grade and optional remark (Grade Roster).
IF COL_LENGTH('dbo.tbl_faculty_class_enrollment', 'official_grade') IS NULL
    ALTER TABLE dbo.tbl_faculty_class_enrollment ADD official_grade NVARCHAR(32) NULL;

IF COL_LENGTH('dbo.tbl_faculty_class_enrollment', 'remarks') IS NULL
    ALTER TABLE dbo.tbl_faculty_class_enrollment ADD remarks NVARCHAR(32) NULL;
