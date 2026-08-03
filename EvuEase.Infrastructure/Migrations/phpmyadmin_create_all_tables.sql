SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

CREATE TABLE IF NOT EXISTS `cache` (
  `key` VARCHAR(255) NOT NULL,
  `value` LONGTEXT NOT NULL,
  `expiration` INT NOT NULL,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `cache_locks` (
  `key` VARCHAR(255) NOT NULL,
  `owner` VARCHAR(255) NOT NULL,
  `expiration` INT NOT NULL,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `failed_jobs` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `uuid` VARCHAR(255) NOT NULL,
  `connection` LONGTEXT NOT NULL,
  `queue` LONGTEXT NOT NULL,
  `payload` LONGTEXT NOT NULL,
  `exception` LONGTEXT NOT NULL,
  `failed_at` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `ux_failed_jobs_uuid` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `jobs` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `queue` VARCHAR(255) NOT NULL,
  `payload` LONGTEXT NOT NULL,
  `attempts` TINYINT UNSIGNED NOT NULL,
  `reserved_at` INT NULL,
  `available_at` INT NOT NULL,
  `created_at` INT NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `job_batches` (
  `id` VARCHAR(255) NOT NULL,
  `name` VARCHAR(255) NOT NULL,
  `total_jobs` INT NOT NULL,
  `pending_jobs` INT NOT NULL,
  `failed_jobs` INT NOT NULL,
  `failed_job_ids` LONGTEXT NOT NULL,
  `options` LONGTEXT NULL,
  `cancelled_at` INT NULL,
  `created_at` INT NOT NULL,
  `finished_at` INT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `migrations` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `migration` VARCHAR(255) NOT NULL,
  `batch` INT NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `password_reset_tokens` (
  `email` VARCHAR(255) NOT NULL,
  `token` VARCHAR(255) NOT NULL,
  `created_at` DATETIME(6) NULL,
  PRIMARY KEY (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `sessions` (
  `id` VARCHAR(255) NOT NULL,
  `user_id` BIGINT NULL,
  `ip_address` VARCHAR(45) NULL,
  `user_agent` LONGTEXT NULL,
  `payload` LONGTEXT NOT NULL,
  `last_activity` INT NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `system_logs` (
  `log_id` BIGINT NOT NULL AUTO_INCREMENT,
  `user` VARCHAR(50) NOT NULL,
  `role` VARCHAR(50) NOT NULL,
  `action` VARCHAR(50) NOT NULL,
  `timestamp` DATETIME(6) NOT NULL,
  `module` VARCHAR(50) NOT NULL,
  `details` VARCHAR(50) NOT NULL,
  `ip_address` VARCHAR(50) NULL,
  PRIMARY KEY (`log_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `users` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) NOT NULL,
  `email` VARCHAR(255) NOT NULL,
  `email_verified_at` DATETIME(6) NULL,
  `password` VARCHAR(255) NOT NULL,
  `remember_token` VARCHAR(100) NULL,
  `role` VARCHAR(255) NOT NULL DEFAULT 'evaluator',
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `ux_users_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_program` (
  `program_id` BIGINT NOT NULL AUTO_INCREMENT,
  `program_code` VARCHAR(50) NOT NULL,
  `program_title` VARCHAR(50) NOT NULL,
  `program_completionyears` INT NOT NULL,
  `program_totalunits` INT NULL,
  `program_status` VARCHAR(50) NOT NULL DEFAULT 'active',
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`program_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_sy_term` (
  `sy_id` BIGINT NOT NULL AUTO_INCREMENT,
  `sy_code` VARCHAR(255) NOT NULL,
  `sy_year` VARCHAR(255) NOT NULL,
  `sy_semester` VARCHAR(255) NOT NULL,
  `sy_startdate` DATE NOT NULL,
  `sy_enddate` DATE NOT NULL,
  `sy_enrollmentstart` DATE NOT NULL,
  `sy_enrollmentend` DATE NOT NULL,
  `sy_status` VARCHAR(10) NOT NULL DEFAULT 'Inactive',
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`sy_id`),
  CONSTRAINT `chk_sy_status` CHECK (`sy_status` IN ('Active', 'Inactive'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_curricula` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `curriculum_code` VARCHAR(50) NOT NULL,
  `version` VARCHAR(50) NOT NULL,
  `program_id` BIGINT NOT NULL,
  `sy_id` BIGINT NOT NULL,
  `effective_date` DATE NOT NULL,
  `curriculum_status` VARCHAR(50) NOT NULL DEFAULT 'Inactive',
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `supporting_document_file_name` VARCHAR(260) NULL,
  `supporting_document_storage_key` VARCHAR(500) NULL,
  `supporting_document_uploaded_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_course` (
  `course_code` VARCHAR(20) NOT NULL,
  `curriculum_id` INT NOT NULL,
  `program_id` INT NOT NULL,
  `course_title` VARCHAR(250) NOT NULL,
  `course_lec_units` INT NOT NULL,
  `course_lab_units` INT NOT NULL,
  `course_total_units` INT NOT NULL,
  `course_yearlevel` VARCHAR(20) NOT NULL,
  `course_semester` VARCHAR(20) NOT NULL,
  `course_component` VARCHAR(100) NULL,
  `prerequisites` VARCHAR(200) NULL,
  `description` LONGTEXT NULL,
  `course_has_prerequities` INT NOT NULL DEFAULT 0,
  `is_elective_slot` TINYINT(1) NOT NULL DEFAULT 0,
  `is_elective_option` TINYINT(1) NOT NULL DEFAULT 0,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`curriculum_id`, `course_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_students` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `student_number` VARCHAR(50) NOT NULL,
  `first_name` VARCHAR(100) NOT NULL,
  `last_name` VARCHAR(100) NOT NULL,
  `middle_name` VARCHAR(100) NULL,
  `program_code` VARCHAR(50) NOT NULL,
  `program_title` VARCHAR(300) NOT NULL,
  `year_level` VARCHAR(50) NOT NULL,
  `student_type` VARCHAR(50) NOT NULL,
  `enrollment_status` VARCHAR(50) NOT NULL,
  `curriculum_code` VARCHAR(50) NULL,
  `address` VARCHAR(500) NULL,
  `contact_number` VARCHAR(50) NULL,
  `email` VARCHAR(200) NULL,
  `gender` VARCHAR(20) NULL,
  `birthdate` DATE NULL,
  `portal_password_hash` VARCHAR(255) NULL,
  `portal_password_must_change` TINYINT(1) NOT NULL DEFAULT 0,
  `portal_password_expires_at` DATETIME(6) NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_student_portal_password_reset_requests` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `student_id` BIGINT NOT NULL,
  `student_number` VARCHAR(50) NOT NULL,
  `reason` VARCHAR(500) NULL,
  `status` VARCHAR(30) NOT NULL,
  `registrar_notes` VARCHAR(500) NULL,
  `resolved_by` VARCHAR(200) NULL,
  `temporary_password` VARCHAR(255) NULL,
  `temporary_password_expires_at` DATETIME(6) NULL,
  `requested_at` DATETIME(6) NOT NULL,
  `resolved_at` DATETIME(6) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_student_curriculum_history` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `student_id` BIGINT NOT NULL,
  `curriculum_code` VARCHAR(50) NOT NULL,
  `effective_school_year` VARCHAR(64) NULL,
  `reason` VARCHAR(500) NULL,
  `notes` VARCHAR(1000) NULL,
  `migrated_by` VARCHAR(200) NULL,
  `created_at` DATETIME(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_tuition_fees` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `sy_id` VARCHAR(50) NULL,
  `batch` VARCHAR(10) NULL,
  `semester` VARCHAR(20) NULL,
  `course_code` VARCHAR(20) NULL,
  `course_title` VARCHAR(255) NULL,
  `component` VARCHAR(50) NULL,
  `units` DECIMAL(5,2) NULL,
  `cash` DECIMAL(18,2) NOT NULL,
  `low_monthly_payment` DECIMAL(18,2) NOT NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_miscellaneous_fees` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `sy_id` VARCHAR(50) NULL,
  `batch` VARCHAR(10) NULL,
  `semester` VARCHAR(20) NULL,
  `miscellaneous_fee` VARCHAR(255) NULL,
  `cash` DECIMAL(18,2) NOT NULL,
  `low_monthly_payment` DECIMAL(18,2) NOT NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_other_school_fees` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `sy_id` VARCHAR(50) NULL,
  `batch` VARCHAR(10) NULL,
  `semester` VARCHAR(20) NULL,
  `school_fee` VARCHAR(255) NULL,
  `cash` DECIMAL(18,2) NOT NULL,
  `low_monthly_payment` DECIMAL(18,2) NOT NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_dp_percentage` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `program_code` VARCHAR(32) NULL,
  `program_title` VARCHAR(200) NULL,
  `batch` VARCHAR(32) NULL,
  `downpayment_percent` DECIMAL(5,2) NOT NULL,
  `effective_school_year` VARCHAR(64) NULL,
  `created_by` VARCHAR(255) NULL,
  `updated_by` VARCHAR(255) NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_payment_scheme` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `school_year` VARCHAR(64) NULL,
  `semester` VARCHAR(32) NULL,
  `description` VARCHAR(500) NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_payment_scheme_installment` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `payment_scheme_id` BIGINT NOT NULL,
  `installment_order` INT NOT NULL,
  `payment_name` VARCHAR(200) NULL,
  `due_date` DATETIME(6) NOT NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`),
  KEY `ix_payment_scheme_installment_scheme_id` (`payment_scheme_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_class_roster` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_grade_roster` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_grading_scheme_basis` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `academic_term_key` VARCHAR(64) NOT NULL,
  `grading_scheme_code` VARCHAR(32) NOT NULL,
  `grading_scheme_description` VARCHAR(200) NOT NULL,
  `grading_basis_code` VARCHAR(32) NOT NULL,
  `grading_basis_description` VARCHAR(200) NOT NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_grading_scheme_basis_academic_term_key` (`academic_term_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_grade_scale_row` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `academic_term_key` VARCHAR(64) NOT NULL,
  `mark` DECIMAL(9,4) NOT NULL,
  `grade` DECIMAL(9,4) NOT NULL,
  `sort_order` INT NOT NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`),
  KEY `ix_grade_scale_row_academic_term_key` (`academic_term_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_faculty_class` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `course_code` VARCHAR(32) NOT NULL,
  `class_number` VARCHAR(32) NOT NULL,
  `section` VARCHAR(64) NOT NULL,
  `course_title` VARCHAR(300) NOT NULL,
  `component` VARCHAR(64) NOT NULL,
  `academic_term` VARCHAR(200) NOT NULL,
  `enrolled_count` INT NOT NULL DEFAULT 0,
  `program_code` VARCHAR(32) NOT NULL,
  `year_level` VARCHAR(64) NOT NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_faculty_class_enrollment` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `faculty_class_id` BIGINT NOT NULL,
  `student_id` BIGINT NOT NULL,
  `official_grade` VARCHAR(32) NULL,
  `remarks` VARCHAR(32) NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UQ_fce_class_student` (`faculty_class_id`, `student_id`),
  KEY `IX_fce_faculty_class_id` (`faculty_class_id`),
  CONSTRAINT `fk_fce_faculty_class`
    FOREIGN KEY (`faculty_class_id`) REFERENCES `tbl_faculty_class` (`id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_fce_student`
    FOREIGN KEY (`student_id`) REFERENCES `tbl_students` (`id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_credit_request` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `credit_request_no` VARCHAR(32) NOT NULL,
  `student_id` BIGINT NULL,
  `student_number` VARCHAR(50) NOT NULL,
  `first_name` VARCHAR(100) NOT NULL,
  `middle_name` VARCHAR(100) NULL,
  `last_name` VARCHAR(100) NOT NULL,
  `program_id` BIGINT NOT NULL,
  `sy_id` BIGINT NOT NULL,
  `request_status` VARCHAR(32) NOT NULL,
  `signed_pdf_file_name` VARCHAR(260) NULL,
  `signed_pdf_storage_key` VARCHAR(500) NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `ux_credit_request_no` (`credit_request_no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_credit_request_line` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `credit_request_id` BIGINT NOT NULL,
  `sort_order` INT NOT NULL,
  `applied_course_code` VARCHAR(32) NULL,
  `applied_course_title` VARCHAR(200) NULL,
  `applied_lec_units` DECIMAL(5,2) NOT NULL,
  `applied_lab_units` DECIMAL(5,2) NOT NULL,
  `grade` VARCHAR(16) NULL,
  `equivalent_course_code` VARCHAR(32) NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`),
  KEY `ix_credit_request_line_request_id` (`credit_request_id`),
  CONSTRAINT `fk_credit_request_line_request`
    FOREIGN KEY (`credit_request_id`) REFERENCES `tbl_credit_request` (`id`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_subject_evaluation_audit` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `student_id` BIGINT NOT NULL,
  `student_number` VARCHAR(50) NOT NULL,
  `student_name` VARCHAR(255) NOT NULL,
  `program_code` VARCHAR(50) NOT NULL,
  `program_year_level` VARCHAR(100) NOT NULL,
  `school_year` VARCHAR(50) NOT NULL,
  `semester` VARCHAR(50) NOT NULL,
  `school_year_term` VARCHAR(200) NOT NULL,
  `total_units_selected` INT NOT NULL,
  `evaluated_by` VARCHAR(128) NOT NULL,
  `evaluated_at` DATETIME(6) NOT NULL,
  `evaluation_payload` LONGTEXT NOT NULL,
  `status` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME(6) NULL,
  `updated_at` DATETIME(6) NULL,
  `deleted_at` DATETIME(6) NULL,
  `deleted_by` VARCHAR(255) NULL,
  PRIMARY KEY (`id`),
  KEY `ix_subject_evaluation_audit_evaluated_at` (`evaluated_at`),
  KEY `ix_subject_evaluation_audit_student_id` (`student_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

SET FOREIGN_KEY_CHECKS = 1;
