-- Check current structure of tbl_tuition_fees table
SELECT 
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.CHARACTER_MAXIMUM_LENGTH,
    c.NUMERIC_PRECISION,
    c.NUMERIC_SCALE,
    c.IS_NULLABLE,
    c.COLUMN_DEFAULT,
    CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'YES' ELSE 'NO' END AS IS_PRIMARY_KEY,
    CASE WHEN ic.name IS NOT NULL THEN 'YES' ELSE 'NO' END AS IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS c
LEFT JOIN (
    SELECT ku.TABLE_NAME, ku.COLUMN_NAME
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
        ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
    WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
        AND ku.TABLE_NAME = 'tbl_tuition_fees'
) pk ON c.COLUMN_NAME = pk.COLUMN_NAME AND c.TABLE_NAME = pk.TABLE_NAME
LEFT JOIN sys.columns sc ON sc.object_id = OBJECT_ID('tbl_tuition_fees') AND sc.name = c.COLUMN_NAME
LEFT JOIN sys.identity_columns ic ON ic.object_id = sc.object_id AND ic.column_id = sc.column_id
WHERE c.TABLE_NAME = 'tbl_tuition_fees'
ORDER BY c.ORDINAL_POSITION;

-- Check if table exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tbl_tuition_fees')
BEGIN
    PRINT 'Table tbl_tuition_fees does not exist. Creating table...';
    
    CREATE TABLE tbl_tuition_fees (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        sy_id NVARCHAR(50) NULL,
        batch NVARCHAR(10) NULL,
        semester NVARCHAR(20) NULL,
        course_code NVARCHAR(20) NULL,
        course_title NVARCHAR(255) NULL,
        component NVARCHAR(50) NULL,
        units DECIMAL(5,2) NULL,
        cash DECIMAL(18,2) NOT NULL DEFAULT 0.00,
        low_monthly_payment DECIMAL(18,2) NOT NULL DEFAULT 0.00,
        status BIT NOT NULL DEFAULT 1,
        created_at DATETIME NULL,
        updated_at DATETIME NULL
    );
    
    PRINT 'Table tbl_tuition_fees created successfully.';
END
ELSE
BEGIN
    PRINT 'Table tbl_tuition_fees exists. Checking for missing columns...';
    
    -- Add sy_id column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'sy_id')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD sy_id NVARCHAR(50) NULL;
        PRINT 'Added column: sy_id';
    END
    
    -- Add batch column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'batch')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD batch NVARCHAR(10) NULL;
        PRINT 'Added column: batch';
    END
    
    -- Add semester column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'semester')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD semester NVARCHAR(20) NULL;
        PRINT 'Added column: semester';
    END
    
    -- Add course_code column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'course_code')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD course_code NVARCHAR(20) NULL;
        PRINT 'Added column: course_code';
    END
    
    -- Add course_title column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'course_title')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD course_title NVARCHAR(255) NULL;
        PRINT 'Added column: course_title';
    END
    
    -- Add component column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'component')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD component NVARCHAR(50) NULL;
        PRINT 'Added column: component';
    END
    
    -- Add units column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'units')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD units DECIMAL(5,2) NULL;
        PRINT 'Added column: units';
    END
    
    -- Add cash column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'cash')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD cash DECIMAL(18,2) NOT NULL DEFAULT 0.00;
        PRINT 'Added column: cash';
    END
    
    -- Add low_monthly_payment column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'low_monthly_payment')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD low_monthly_payment DECIMAL(18,2) NOT NULL DEFAULT 0.00;
        PRINT 'Added column: low_monthly_payment';
    END
    
    -- Add status column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'status')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD status BIT NOT NULL DEFAULT 1;
        PRINT 'Added column: status';
    END
    
    -- Add created_at column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'created_at')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD created_at DATETIME NULL;
        PRINT 'Added column: created_at';
    END
    
    -- Add updated_at column if missing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'updated_at')
    BEGIN
        ALTER TABLE tbl_tuition_fees ADD updated_at DATETIME NULL;
        PRINT 'Added column: updated_at';
    END
    
    -- Ensure id column is identity if table exists but id is not identity
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'id')
        AND NOT EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID('tbl_tuition_fees') AND name = 'id')
    BEGIN
        PRINT 'Warning: id column exists but is not an IDENTITY column. Manual intervention may be required.';
    END
    
    PRINT 'Column check completed.';
END

-- Final verification: Show all columns in the table
PRINT '';
PRINT '=== Final Table Structure ===';
SELECT 
    COLUMN_NAME,
    DATA_TYPE + 
    CASE 
        WHEN DATA_TYPE IN ('nvarchar', 'varchar', 'char', 'nchar') 
            THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
        WHEN DATA_TYPE IN ('decimal', 'numeric')
            THEN '(' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR) + ')'
        ELSE ''
    END AS DATA_TYPE_FULL,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    CASE WHEN COLUMNPROPERTY(OBJECT_ID('tbl_tuition_fees'), COLUMN_NAME, 'IsIdentity') = 1 THEN 'YES' ELSE 'NO' END AS IS_IDENTITY
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'tbl_tuition_fees'
ORDER BY ORDINAL_POSITION;


