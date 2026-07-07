-- Expand course title column to support up to 250 characters.
IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'tbl_course')
      AND name = N'course_title'
)
BEGIN
    ALTER TABLE tbl_course ALTER COLUMN course_title NVARCHAR(250) NOT NULL;
END
