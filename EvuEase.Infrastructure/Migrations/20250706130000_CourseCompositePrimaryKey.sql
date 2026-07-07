-- Course codes are unique per curriculum version, not globally.
IF OBJECT_ID(N'tbl_course', N'U') IS NULL
BEGIN
    RETURN;
END

DECLARE @pkName NVARCHAR(256);
SELECT @pkName = kc.name
FROM sys.key_constraints kc
WHERE kc.parent_object_id = OBJECT_ID(N'tbl_course')
  AND kc.type = 'PK';

IF @pkName IS NOT NULL
BEGIN
    EXEC(N'ALTER TABLE tbl_course DROP CONSTRAINT [' + @pkName + N']');
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE parent_object_id = OBJECT_ID(N'tbl_course')
      AND name = N'PK_tbl_course_curriculum_course_code'
)
BEGIN
    ALTER TABLE tbl_course
        ADD CONSTRAINT PK_tbl_course_curriculum_course_code
        PRIMARY KEY (curriculum_id, course_code);
END
