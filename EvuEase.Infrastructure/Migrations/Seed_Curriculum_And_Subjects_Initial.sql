-- ============================================================
-- Seed initial curriculum & subject data (EvuEase)
-- Idempotent: safe to run multiple times (skips existing rows)
--
-- Prerequisites:
--   - tbl_program should have BSCS and BSIT (script can create them)
--   - tbl_sy_term should have at least one row (script can create one)
--
-- Includes sample elective slots + eligible elective options for testing.
-- ============================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @now DATETIME2 = SYSDATETIME();

/* ---------- Optional: minimal programs ---------- */
IF NOT EXISTS (SELECT 1 FROM tbl_program WHERE program_code = 'BSCS')
BEGIN
    INSERT INTO tbl_program (program_code, program_title, program_completionyears, program_totalunits, program_status, status, created_at)
    VALUES ('BSCS', 'Bachelor of Science in Computer Science', 4, 144, 'active', 1, @now);
    PRINT 'Inserted program: BSCS';
END

IF NOT EXISTS (SELECT 1 FROM tbl_program WHERE program_code = 'BSIT')
BEGIN
    INSERT INTO tbl_program (program_code, program_title, program_completionyears, program_totalunits, program_status, status, created_at)
    VALUES ('BSIT', 'Bachelor of Science in Information Technology', 4, 144, 'active', 1, @now);
    PRINT 'Inserted program: BSIT';
END

DECLARE @bscsProgramId BIGINT = (SELECT TOP 1 program_id FROM tbl_program WHERE program_code = 'BSCS' ORDER BY program_id);
DECLARE @bsitProgramId BIGINT = (SELECT TOP 1 program_id FROM tbl_program WHERE program_code = 'BSIT' ORDER BY program_id);

IF @bscsProgramId IS NULL OR @bsitProgramId IS NULL
BEGIN
    RAISERROR('BSCS and BSIT programs are required. Insert them in tbl_program first.', 16, 1);
    ROLLBACK TRANSACTION;
    RETURN;
END

/* ---------- Optional: minimal school year / term ---------- */
IF NOT EXISTS (SELECT 1 FROM tbl_sy_term)
BEGIN
    INSERT INTO tbl_sy_term (
        sy_code, sy_year, sy_semester,
        sy_startdate, sy_enddate,
        sy_enrollmentstart, sy_enrollmentend,
        sy_status, status, created_at
    )
    VALUES (
        '2024-2025-1ST', '2024-2025', '1st Semester',
        '2024-08-01', '2024-12-15',
        '2024-07-01', '2024-08-15',
        'Active', 1, @now
    );
    PRINT 'Inserted default school year term.';
END

DECLARE @syId BIGINT = (
    SELECT TOP 1 sy_id
    FROM tbl_sy_term
    ORDER BY CASE WHEN sy_status = 'Active' THEN 0 ELSE 1 END, sy_id DESC
);

IF @syId IS NULL
BEGIN
    RAISERROR('At least one row in tbl_sy_term is required.', 16, 1);
    ROLLBACK TRANSACTION;
    RETURN;
END

/* ---------- Curricula ---------- */
IF NOT EXISTS (SELECT 1 FROM tbl_curricula WHERE curriculum_code = 'BSCS-24-01')
BEGIN
    INSERT INTO tbl_curricula (
        curriculum_code, version, program_id, sy_id,
        effective_date, curriculum_status, status, created_at
    )
    VALUES ('BSCS-24-01', '24-01', @bscsProgramId, @syId, '2024-08-01', 'Active', 1, @now);
    PRINT 'Inserted curriculum: BSCS-24-01';
END

IF NOT EXISTS (SELECT 1 FROM tbl_curricula WHERE curriculum_code = 'BSCS-25-01')
BEGIN
    INSERT INTO tbl_curricula (
        curriculum_code, version, program_id, sy_id,
        effective_date, curriculum_status, status, created_at
    )
    VALUES ('BSCS-25-01', '25-01', @bscsProgramId, @syId, '2025-08-01', 'Active', 1, @now);
    PRINT 'Inserted curriculum: BSCS-25-01';
END

IF NOT EXISTS (SELECT 1 FROM tbl_curricula WHERE curriculum_code = 'BSIT-24-01')
BEGIN
    INSERT INTO tbl_curricula (
        curriculum_code, version, program_id, sy_id,
        effective_date, curriculum_status, status, created_at
    )
    VALUES ('BSIT-24-01', '24-01', @bsitProgramId, @syId, '2024-08-01', 'Active', 1, @now);
    PRINT 'Inserted curriculum: BSIT-24-01';
END

IF NOT EXISTS (SELECT 1 FROM tbl_curricula WHERE curriculum_code = 'BSIT-25-01')
BEGIN
    INSERT INTO tbl_curricula (
        curriculum_code, version, program_id, sy_id,
        effective_date, curriculum_status, status, created_at
    )
    VALUES ('BSIT-25-01', '25-01', @bsitProgramId, @syId, '2025-08-01', 'Active', 1, @now);
    PRINT 'Inserted curriculum: BSIT-25-01';
END

DECLARE @bscs24CurriculumId BIGINT = (SELECT id FROM tbl_curricula WHERE curriculum_code = 'BSCS-24-01');
DECLARE @bsit24CurriculumId BIGINT = (SELECT id FROM tbl_curricula WHERE curriculum_code = 'BSIT-24-01');

/* ---------- BSCS-24-01 subjects (Year 1 sample + electives) ---------- */
IF NOT EXISTS (SELECT 1 FROM tbl_course WHERE course_code = 'COSC1001')
BEGIN
    INSERT INTO tbl_course (
        course_code, curriculum_id, program_id, course_title,
        course_lec_units, course_lab_units, course_total_units,
        course_yearlevel, course_semester, course_component,
        prerequisites, description, course_has_prerequities,
        is_elective_slot, is_elective_option, status, created_at
    )
    VALUES
    ('COSC1001', @bscs24CurriculumId, @bscsProgramId, 'Introduction to Computing', 2, 1, 3, 'Year 1', '1st Semester', 'Lecture, Lab', 'None', 'Introduction to Computing', 0, 0, 0, 1, @now),
    ('COSC1002', @bscs24CurriculumId, @bscsProgramId, 'Computer Programming 1', 2, 1, 3, 'Year 1', '1st Semester', 'Lecture, Lab', 'None', 'Computer Programming 1', 0, 0, 0, 1, @now),
    ('GEDC1001', @bscs24CurriculumId, @bscsProgramId, 'Understanding the Self', 3, 0, 3, 'Year 1', '1st Semester', 'Lecture', 'None', 'Understanding the Self', 0, 0, 0, 1, @now),
    ('GEDC1002', @bscs24CurriculumId, @bscsProgramId, 'Readings in Philippine History', 3, 0, 3, 'Year 1', '1st Semester', 'Lecture', 'None', 'Readings in Philippine History', 0, 0, 0, 1, @now),
    ('NSTP1001', @bscs24CurriculumId, @bscsProgramId, 'National Service Training Program 1', 3, 0, 3, 'Year 1', '1st Semester', 'Lecture', 'None', 'NSTP 1', 0, 0, 0, 1, @now),
    ('PHED1001', @bscs24CurriculumId, @bscsProgramId, 'Physical Education 1', 2, 0, 2, 'Year 1', '1st Semester', 'Lecture', 'None', 'Physical Education 1', 0, 0, 0, 1, @now),
    ('ELECSLOT1', @bscs24CurriculumId, @bscsProgramId, 'CS Elective 1', 3, 0, 3, 'Year 1', '2nd Semester', 'Lecture', 'None', 'CS Elective 1 placeholder', 0, 1, 0, 1, @now),
    ('ELECSLOT2', @bscs24CurriculumId, @bscsProgramId, 'CS Elective 2', 3, 0, 3, 'Year 2', '1st Semester', 'Lecture', 'COSC1002', 'CS Elective 2 placeholder', 1, 1, 0, 1, @now),
    ('COSC2001', @bscs24CurriculumId, @bscsProgramId, 'Data Structures and Algorithms', 2, 1, 3, 'Year 2', '1st Semester', 'Lecture, Lab', 'COSC1002', 'Data Structures and Algorithms', 1, 0, 0, 1, @now),
    ('CSELEC101', @bscs24CurriculumId, @bscsProgramId, 'Game Development', 2, 1, 3, 'Year 2', '1st Semester', 'Lecture, Lab', 'COSC1002', 'Game Development elective', 1, 0, 1, 1, @now),
    ('CSELEC102', @bscs24CurriculumId, @bscsProgramId, 'Mobile Application Development', 2, 1, 3, 'Year 2', '1st Semester', 'Lecture, Lab', 'COSC1002', 'Mobile Application Development elective', 1, 0, 1, 1, @now),
    ('CSELEC103', @bscs24CurriculumId, @bscsProgramId, 'Artificial Intelligence', 3, 0, 3, 'Year 3', '1st Semester', 'Lecture', 'COSC2001', 'Artificial Intelligence elective', 1, 0, 1, 1, @now);
    PRINT 'Inserted BSCS-24-01 sample courses.';
END

/* ---------- BSIT-24-01 subjects (Year 1 sample + electives) ---------- */
IF NOT EXISTS (SELECT 1 FROM tbl_course WHERE course_code = 'CITE1001')
BEGIN
    INSERT INTO tbl_course (
        course_code, curriculum_id, program_id, course_title,
        course_lec_units, course_lab_units, course_total_units,
        course_yearlevel, course_semester, course_component,
        prerequisites, description, course_has_prerequities,
        is_elective_slot, is_elective_option, status, created_at
    )
    VALUES
    ('CITE1001', @bsit24CurriculumId, @bsitProgramId, 'Introduction to Computing', 2, 1, 3, 'Year 1', '1st Semester', 'Lecture, Lab', 'None', 'Introduction to Computing', 0, 0, 0, 1, @now),
    ('CITE1002', @bsit24CurriculumId, @bsitProgramId, 'Computer Programming 1', 2, 1, 3, 'Year 1', '1st Semester', 'Lecture, Lab', 'None', 'Computer Programming 1', 0, 0, 0, 1, @now),
    ('GEDC1003', @bsit24CurriculumId, @bsitProgramId, 'Mathematics in the Modern World', 3, 0, 3, 'Year 1', '1st Semester', 'Lecture', 'None', 'Mathematics in the Modern World', 0, 0, 0, 1, @now),
    ('ELECSLOT3', @bsit24CurriculumId, @bsitProgramId, 'IT Elective 1', 3, 0, 3, 'Year 2', '1st Semester', 'Lecture', 'CITE1002', 'IT Elective 1 placeholder', 1, 1, 0, 1, @now),
    ('ITELEC101', @bsit24CurriculumId, @bsitProgramId, 'Web Systems and Technologies', 2, 1, 3, 'Year 2', '1st Semester', 'Lecture, Lab', 'CITE1002', 'Web Systems elective', 1, 0, 1, 1, @now),
    ('ITELEC102', @bsit24CurriculumId, @bsitProgramId, 'Network Administration', 2, 1, 3, 'Year 2', '1st Semester', 'Lecture, Lab', 'CITE1002', 'Network Administration elective', 1, 0, 1, 1, @now);
    PRINT 'Inserted BSIT-24-01 sample courses.';
END

/* ---------- Optional: sample tuition fees for charge slip testing ---------- */
IF OBJECT_ID('tbl_tuition_fees', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM tbl_tuition_fees WHERE course_code = 'COSC1001')
BEGIN
    INSERT INTO tbl_tuition_fees (
        sy_id, batch, semester, course_code, course_title,
        component, units, cash, low_monthly_payment, status, created_at
    )
    VALUES
    (CAST(@syId AS NVARCHAR(50)), 'A', '1st Semester', 'COSC1001', 'Introduction to Computing', 'Lecture, Lab', 3, 4500.00, 500.00, 1, @now),
    (CAST(@syId AS NVARCHAR(50)), 'A', '1st Semester', 'COSC1002', 'Computer Programming 1', 'Lecture, Lab', 3, 4500.00, 500.00, 1, @now),
    (CAST(@syId AS NVARCHAR(50)), 'A', '1st Semester', 'CSELEC101', 'Game Development', 'Lecture, Lab', 3, 4800.00, 520.00, 1, @now),
    (CAST(@syId AS NVARCHAR(50)), 'A', '1st Semester', 'CSELEC102', 'Mobile Application Development', 'Lecture, Lab', 3, 4800.00, 520.00, 1, @now),
    (CAST(@syId AS NVARCHAR(50)), 'A', '1st Semester', 'CITE1001', 'Introduction to Computing', 'Lecture, Lab', 3, 4500.00, 500.00, 1, @now),
    (CAST(@syId AS NVARCHAR(50)), 'A', '1st Semester', 'ITELEC101', 'Web Systems and Technologies', 'Lecture, Lab', 3, 4800.00, 520.00, 1, @now);
    PRINT 'Inserted sample tuition fees.';
END

COMMIT TRANSACTION;

/* ---------- Verification ---------- */
SELECT curriculum_code, version, program_id, sy_id, effective_date, curriculum_status, status
FROM tbl_curricula
WHERE curriculum_code IN ('BSCS-24-01', 'BSCS-25-01', 'BSIT-24-01', 'BSIT-25-01')
ORDER BY curriculum_code;

SELECT
    c.course_code,
    cu.curriculum_code,
    c.course_title,
    c.course_yearlevel,
    c.course_semester,
    c.course_total_units,
    c.is_elective_slot,
    c.is_elective_option
FROM tbl_course c
INNER JOIN tbl_curricula cu ON cu.id = c.curriculum_id
WHERE cu.curriculum_code IN ('BSCS-24-01', 'BSIT-24-01')
ORDER BY cu.curriculum_code, c.course_yearlevel, c.course_semester, c.course_code;

GO
