-- =====================================================================
-- Car Care Tracker - SQL Server Database Script
-- Project: Vehicle Expense and Maintenance Management System
-- Students: Jawad Alshamaileh (202230091), Omar Subhi Abu Aladass (202311250)
-- Supervisor: Dr. Alaa Abuthawabeh
-- Amman Arab University - College of IT
-- =====================================================================

-- ============================================
-- STEP 0: CREATE DATABASE (run this part first)
-- ============================================
IF DB_ID('CarCareTracker') IS NULL
    CREATE DATABASE CarCareTracker;
GO

USE CarCareTracker;
GO

-- ============================================
-- STEP 1: DROP OLD TABLES (if exist) - safe re-run
-- Order matters because of foreign keys.
-- ============================================
IF OBJECT_ID('V_VEHICLE_TOTAL_EXPENSES','V') IS NOT NULL DROP VIEW V_VEHICLE_TOTAL_EXPENSES;
IF OBJECT_ID('V_FUEL_TOTALS','V') IS NOT NULL DROP VIEW V_FUEL_TOTALS;
IF OBJECT_ID('V_MAINT_TOTALS','V') IS NOT NULL DROP VIEW V_MAINT_TOTALS;
GO
IF OBJECT_ID('RECEIPTS','U') IS NOT NULL DROP TABLE RECEIPTS;
IF OBJECT_ID('REMINDERS','U') IS NOT NULL DROP TABLE REMINDERS;
IF OBJECT_ID('MAINTENANCE_RECORDS','U') IS NOT NULL DROP TABLE MAINTENANCE_RECORDS;
IF OBJECT_ID('FUEL_RECORDS','U') IS NOT NULL DROP TABLE FUEL_RECORDS;
IF OBJECT_ID('VEHICLES','U') IS NOT NULL DROP TABLE VEHICLES;
IF OBJECT_ID('SERVICE_TYPES','U') IS NOT NULL DROP TABLE SERVICE_TYPES;
IF OBJECT_ID('VEHICLE_TYPES','U') IS NOT NULL DROP TABLE VEHICLE_TYPES;
IF OBJECT_ID('USERS','U') IS NOT NULL DROP TABLE USERS;
IF OBJECT_ID('ROLES','U') IS NOT NULL DROP TABLE ROLES;
GO

-- ============================================
-- STEP 2: CREATE TABLES
-- ============================================

-- 1) ROLES Table
CREATE TABLE ROLES (
    role_id     INT IDENTITY(1,1) PRIMARY KEY,
    role_name   NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- 2) USERS Table
CREATE TABLE USERS (
    user_id        INT IDENTITY(1,1) PRIMARY KEY,
    full_name      NVARCHAR(100) NOT NULL,
    email          NVARCHAR(100) NOT NULL UNIQUE,
    password_hash  NVARCHAR(255) NOT NULL,
    phone          NVARCHAR(20),
    role_id        INT NOT NULL,
    is_active      BIT DEFAULT 1 NOT NULL,
    created_at     DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_users_role FOREIGN KEY (role_id) REFERENCES ROLES(role_id)
);
GO

-- 3) VEHICLE_TYPES Table
CREATE TABLE VEHICLE_TYPES (
    type_id     INT IDENTITY(1,1) PRIMARY KEY,
    type_name   NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- 4) VEHICLES Table
CREATE TABLE VEHICLES (
    vehicle_id        INT IDENTITY(1,1) PRIMARY KEY,
    user_id           INT NOT NULL,
    type_id           INT NOT NULL,
    plate_number      NVARCHAR(20) NOT NULL UNIQUE,
    brand             NVARCHAR(50) NOT NULL,
    model             NVARCHAR(50) NOT NULL,
    year_made         INT NOT NULL,
    color             NVARCHAR(30),
    current_odometer  BIGINT DEFAULT 0,
    created_at        DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_vehicles_user FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT fk_vehicles_type FOREIGN KEY (type_id) REFERENCES VEHICLE_TYPES(type_id),
    CONSTRAINT chk_year CHECK (year_made BETWEEN 1950 AND 2030)
);
GO

-- 5) SERVICE_TYPES Table
CREATE TABLE SERVICE_TYPES (
    service_type_id         INT IDENTITY(1,1) PRIMARY KEY,
    service_name            NVARCHAR(100) NOT NULL UNIQUE,
    default_interval_km     INT,
    default_interval_months INT
);
GO

-- 6) FUEL_RECORDS Table
CREATE TABLE FUEL_RECORDS (
    fuel_id           INT IDENTITY(1,1) PRIMARY KEY,
    vehicle_id        INT NOT NULL,
    fuel_date         DATE NOT NULL,
    liters            DECIMAL(7,2) NOT NULL,
    cost              DECIMAL(10,2) NOT NULL,
    odometer_reading  BIGINT NOT NULL,
    station_name      NVARCHAR(100),
    notes             NVARCHAR(500),
    created_at        DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_fuel_vehicle FOREIGN KEY (vehicle_id) REFERENCES VEHICLES(vehicle_id) ON DELETE CASCADE,
    CONSTRAINT chk_fuel_cost CHECK (cost >= 0),
    CONSTRAINT chk_fuel_liters CHECK (liters > 0)
);
GO

-- 7) MAINTENANCE_RECORDS Table
CREATE TABLE MAINTENANCE_RECORDS (
    maintenance_id    INT IDENTITY(1,1) PRIMARY KEY,
    vehicle_id        INT NOT NULL,
    service_type_id   INT NOT NULL,
    maintenance_date  DATE NOT NULL,
    cost              DECIMAL(10,2) NOT NULL,
    odometer_reading  BIGINT,
    description       NVARCHAR(500),
    garage_name       NVARCHAR(100),
    created_at        DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_maint_vehicle FOREIGN KEY (vehicle_id) REFERENCES VEHICLES(vehicle_id) ON DELETE CASCADE,
    CONSTRAINT fk_maint_service FOREIGN KEY (service_type_id) REFERENCES SERVICE_TYPES(service_type_id),
    CONSTRAINT chk_maint_cost CHECK (cost >= 0)
);
GO

-- 8) REMINDERS Table
CREATE TABLE REMINDERS (
    reminder_id      INT IDENTITY(1,1) PRIMARY KEY,
    vehicle_id       INT NOT NULL,
    service_type_id  INT,
    reminder_date    DATE NOT NULL,
    title            NVARCHAR(100) NOT NULL,
    status           NVARCHAR(20) DEFAULT 'Pending' NOT NULL,
    notes            NVARCHAR(500),
    created_at       DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_rem_vehicle FOREIGN KEY (vehicle_id) REFERENCES VEHICLES(vehicle_id) ON DELETE CASCADE,
    CONSTRAINT fk_rem_service FOREIGN KEY (service_type_id) REFERENCES SERVICE_TYPES(service_type_id),
    CONSTRAINT chk_status CHECK (status IN ('Pending','Completed','Cancelled'))
);
GO

-- 9) RECEIPTS Table
CREATE TABLE RECEIPTS (
    receipt_id    INT IDENTITY(1,1) PRIMARY KEY,
    record_type   NVARCHAR(20) NOT NULL,
    record_id     INT NOT NULL,
    file_path     NVARCHAR(500) NOT NULL,
    uploaded_at   DATETIME DEFAULT GETDATE(),
    CONSTRAINT chk_record_type CHECK (record_type IN ('Fuel','Maintenance'))
);
GO

-- ============================================
-- STEP 3: INSERT INITIAL DATA
-- ============================================

-- Roles
INSERT INTO ROLES (role_name) VALUES ('Admin');
INSERT INTO ROLES (role_name) VALUES ('User');

-- Vehicle Types
INSERT INTO VEHICLE_TYPES (type_name) VALUES ('Sedan');
INSERT INTO VEHICLE_TYPES (type_name) VALUES ('SUV');
INSERT INTO VEHICLE_TYPES (type_name) VALUES ('Hatchback');
INSERT INTO VEHICLE_TYPES (type_name) VALUES ('Pickup Truck');
INSERT INTO VEHICLE_TYPES (type_name) VALUES ('Coupe');
INSERT INTO VEHICLE_TYPES (type_name) VALUES ('Van');
INSERT INTO VEHICLE_TYPES (type_name) VALUES ('Motorcycle');

-- Service Types
INSERT INTO SERVICE_TYPES (service_name, default_interval_km, default_interval_months) VALUES ('Oil Change', 5000, 6);
INSERT INTO SERVICE_TYPES (service_name, default_interval_km, default_interval_months) VALUES ('Tire Rotation', 10000, 12);
INSERT INTO SERVICE_TYPES (service_name, default_interval_km, default_interval_months) VALUES ('Brake Inspection', 20000, 12);
INSERT INTO SERVICE_TYPES (service_name, default_interval_km, default_interval_months) VALUES ('Air Filter', 15000, 12);
INSERT INTO SERVICE_TYPES (service_name, default_interval_km, default_interval_months) VALUES ('Battery Check', NULL, 12);
INSERT INTO SERVICE_TYPES (service_name, default_interval_km, default_interval_months) VALUES ('Insurance Renewal', NULL, 12);
INSERT INTO SERVICE_TYPES (service_name, default_interval_km, default_interval_months) VALUES ('Registration Renewal', NULL, 12);
INSERT INTO SERVICE_TYPES (service_name, default_interval_km, default_interval_months) VALUES ('General Inspection', NULL, 12);
INSERT INTO SERVICE_TYPES (service_name, default_interval_km, default_interval_months) VALUES ('Tire Replacement', 60000, 48);
GO

-- ============================================
-- NOTE ABOUT USERS:
-- We do NOT seed users with a fixed password here, because passwords are
-- hashed with BCrypt INSIDE the application (and a hash can't be hand-typed).
--
-- HOW TO CREATE YOUR FIRST ADMIN:
--   1) Run the app, open the Register page, and create an account
--      (every new account is created with role = User).
--   2) Then promote that account to Admin by running this line in SSMS
--      (replace the email with the one you registered):
--
--        UPDATE USERS SET role_id = 1 WHERE email = 'your-email@example.com';
--
--   3) Log out and log back in - you are now an Admin and the Admin menu appears.
-- ============================================

-- ============================================
-- STEP 4: HELPFUL VIEWS
-- ============================================
GO
CREATE VIEW V_FUEL_TOTALS AS
SELECT v.vehicle_id, v.plate_number, v.brand, v.model,
       ISNULL(SUM(f.cost), 0) AS total_fuel_cost,
       COUNT(f.fuel_id) AS fuel_entries
FROM VEHICLES v
LEFT JOIN FUEL_RECORDS f ON v.vehicle_id = f.vehicle_id
GROUP BY v.vehicle_id, v.plate_number, v.brand, v.model;
GO

CREATE VIEW V_MAINT_TOTALS AS
SELECT v.vehicle_id, v.plate_number, v.brand, v.model,
       ISNULL(SUM(m.cost), 0) AS total_maint_cost,
       COUNT(m.maintenance_id) AS maint_entries
FROM VEHICLES v
LEFT JOIN MAINTENANCE_RECORDS m ON v.vehicle_id = m.vehicle_id
GROUP BY v.vehicle_id, v.plate_number, v.brand, v.model;
GO

CREATE VIEW V_VEHICLE_TOTAL_EXPENSES AS
SELECT v.vehicle_id, v.plate_number, v.brand, v.model,
       ISNULL(f.total_fuel_cost, 0) AS total_fuel,
       ISNULL(m.total_maint_cost, 0) AS total_maintenance,
       ISNULL(f.total_fuel_cost, 0) + ISNULL(m.total_maint_cost, 0) AS total_expenses
FROM VEHICLES v
LEFT JOIN V_FUEL_TOTALS f ON v.vehicle_id = f.vehicle_id
LEFT JOIN V_MAINT_TOTALS m ON v.vehicle_id = m.vehicle_id;
GO

-- ============================================
-- VERIFY: Quick check queries (uncomment to run)
-- ============================================
-- SELECT * FROM ROLES;
-- SELECT * FROM VEHICLE_TYPES;
-- SELECT * FROM SERVICE_TYPES;
-- SELECT name FROM sys.tables ORDER BY name;

-- =====================================================================
-- END OF SCRIPT
-- =====================================================================
