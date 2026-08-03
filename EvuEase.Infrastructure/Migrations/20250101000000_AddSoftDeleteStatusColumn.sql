-- Migration: Add Soft Delete Status Column to All BaseEntity Tables
-- Description: Adds a boolean 'status' column to all tables that extend BaseEntity for soft deletion functionality
-- Date: 2025-01-01

-- Add status column to tbl_program
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_program') AND name = 'status')
BEGIN
    ALTER TABLE tbl_program ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_sy_term
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_sy_term') AND name = 'status')
BEGIN
    ALTER TABLE tbl_sy_term ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to users (if not already exists as boolean)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('users') AND name = 'status' AND system_type_id != 104) -- 104 is BIT type
BEGIN
    ALTER TABLE users ALTER COLUMN status BIT NOT NULL;
    ALTER TABLE users ADD CONSTRAINT DF_users_status DEFAULT 1 FOR status;
END
ELSE IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('users') AND name = 'status')
BEGIN
    ALTER TABLE users ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_course
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_course') AND name = 'status')
BEGIN
    ALTER TABLE tbl_course ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_students
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_students') AND name = 'status')
BEGIN
    ALTER TABLE tbl_students ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_curricula
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_curricula') AND name = 'status')
BEGIN
    ALTER TABLE tbl_curricula ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_class_roster
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_class_roster') AND name = 'status')
BEGIN
    ALTER TABLE tbl_class_roster ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_grade_roster
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_grade_roster') AND name = 'status')
BEGIN
    ALTER TABLE tbl_grade_roster ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_tuition_fees
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'status')
BEGIN
    ALTER TABLE tbl_tuition_fees ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_miscellaneous_fees
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_miscellaneous_fees') AND name = 'status')
BEGIN
    ALTER TABLE tbl_miscellaneous_fees ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_other_school_fees
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_other_school_fees') AND name = 'status')
BEGIN
    ALTER TABLE tbl_other_school_fees ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add status column to tbl_dp_percentage
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_dp_percentage') AND name = 'status')
BEGIN
    ALTER TABLE tbl_dp_percentage ADD status BIT NOT NULL DEFAULT 1;
END
GO

-- Add new columns to tbl_curricula for curriculum management
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_curricula') AND name = 'curriculum_code')
BEGIN
    ALTER TABLE tbl_curricula ADD curriculum_code NVARCHAR(50) NOT NULL DEFAULT '';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_curricula') AND name = 'version')
BEGIN
    ALTER TABLE tbl_curricula ADD version NVARCHAR(50) NOT NULL DEFAULT '';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_curricula') AND name = 'program_id')
BEGIN
    ALTER TABLE tbl_curricula ADD program_id BIGINT NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_curricula') AND name = 'sy_id')
BEGIN
    ALTER TABLE tbl_curricula ADD sy_id BIGINT NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_curricula') AND name = 'effective_date')
BEGIN
    ALTER TABLE tbl_curricula ADD effective_date DATE NOT NULL DEFAULT GETDATE();
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_curricula') AND name = 'curriculum_status')
BEGIN
    ALTER TABLE tbl_curricula ADD curriculum_status NVARCHAR(50) NOT NULL DEFAULT 'Inactive';
END
GO

-- Update existing records to have status = 1 (active) by default
UPDATE tbl_program SET status = 1 WHERE status IS NULL;
UPDATE tbl_sy_term SET status = 1 WHERE status IS NULL;
UPDATE users SET status = 1 WHERE status IS NULL;
UPDATE tbl_course SET status = 1 WHERE status IS NULL;
UPDATE tbl_students SET status = 1 WHERE status IS NULL;
UPDATE tbl_curricula SET status = 1 WHERE status IS NULL;
UPDATE tbl_class_roster SET status = 1 WHERE status IS NULL;
UPDATE tbl_grade_roster SET status = 1 WHERE status IS NULL;
UPDATE tbl_tuition_fees SET status = 1 WHERE status IS NULL;
UPDATE tbl_miscellaneous_fees SET status = 1 WHERE status IS NULL;
UPDATE tbl_other_school_fees SET status = 1 WHERE status IS NULL;
UPDATE tbl_dp_percentage SET status = 1 WHERE status IS NULL;
GO

