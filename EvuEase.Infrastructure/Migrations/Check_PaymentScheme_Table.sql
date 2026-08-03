IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tbl_payment_scheme')
BEGIN
    CREATE TABLE tbl_payment_scheme (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        school_year NVARCHAR(64) NOT NULL,
        semester NVARCHAR(32) NOT NULL,
        description NVARCHAR(500) NULL,
        status BIT NOT NULL CONSTRAINT DF_tbl_payment_scheme_status DEFAULT 1,
        created_at DATETIME2 NULL,
        updated_at DATETIME2 NULL,
        deleted_at DATETIME2 NULL,
        deleted_by NVARCHAR(255) NULL
    );

    CREATE UNIQUE INDEX UX_tbl_payment_scheme_sy_semester_active
        ON tbl_payment_scheme (school_year, semester)
        WHERE status = 1 AND deleted_at IS NULL;
END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tbl_payment_scheme_installment')
BEGIN
    CREATE TABLE tbl_payment_scheme_installment (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        payment_scheme_id BIGINT NOT NULL,
        installment_order INT NOT NULL,
        payment_name NVARCHAR(200) NOT NULL,
        due_date DATE NOT NULL,
        status BIT NOT NULL CONSTRAINT DF_tbl_payment_scheme_installment_status DEFAULT 1,
        created_at DATETIME2 NULL,
        updated_at DATETIME2 NULL,
        deleted_at DATETIME2 NULL,
        deleted_by NVARCHAR(255) NULL,
        CONSTRAINT FK_tbl_payment_scheme_installment_scheme
            FOREIGN KEY (payment_scheme_id) REFERENCES tbl_payment_scheme(id)
    );

    CREATE INDEX IX_tbl_payment_scheme_installment_scheme
        ON tbl_payment_scheme_installment (payment_scheme_id);
END
