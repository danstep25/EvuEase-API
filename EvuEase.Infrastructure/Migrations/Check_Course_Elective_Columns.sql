-- Batch 1: add columns (must be separate from UPDATE — SQL Server validates column names at compile time)
IF OBJECT_ID('tbl_course', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_course') AND name = 'is_elective_slot')
    BEGIN
        ALTER TABLE tbl_course ADD is_elective_slot BIT NOT NULL CONSTRAINT DF_tbl_course_is_elective_slot DEFAULT 0;
        PRINT 'Added column: is_elective_slot';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_course') AND name = 'is_elective_option')
    BEGIN
        ALTER TABLE tbl_course ADD is_elective_option BIT NOT NULL CONSTRAINT DF_tbl_course_is_elective_option DEFAULT 0;
        PRINT 'Added column: is_elective_option';
    END
END
ELSE
BEGIN
    PRINT 'tbl_course does not exist — skipped elective column migration.';
END
GO

-- Batch 2: backfill (runs after columns exist)
IF OBJECT_ID('tbl_course', 'U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_course') AND name = 'is_elective_slot')
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_course') AND name = 'is_elective_option')
BEGIN
    UPDATE tbl_course
    SET is_elective_slot = 1
    WHERE is_elective_slot = 0
      AND (
            course_title LIKE '%Elective%'
            OR course_code LIKE 'ELEC%'
          )
      AND is_elective_option = 0;

    PRINT 'Backfilled elective slot flags where applicable.';
END
GO
