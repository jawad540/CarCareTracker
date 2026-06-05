using System;

namespace CarCareTracker.Models
{
    // ===== ROLES =====
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    // ===== USERS =====
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }   // joined
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ===== VEHICLE TYPES =====
    public class VehicleType
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }

    // ===== VEHICLES =====
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public int UserId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }   // joined
        public string PlateNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearMade { get; set; }
        public string Color { get; set; }
        public long CurrentOdometer { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ===== SERVICE TYPES =====
    public class ServiceType
    {
        public int ServiceTypeId { get; set; }
        public string ServiceName { get; set; }
        public int? DefaultIntervalKm { get; set; }
        public int? DefaultIntervalMonths { get; set; }
    }

    // ===== FUEL RECORDS =====
    public class FuelRecord
    {
        public int FuelId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleLabel { get; set; }  // joined "Brand Model (Plate)"
        public DateTime FuelDate { get; set; }
        public decimal Liters { get; set; }
        public decimal Cost { get; set; }
        public long OdometerReading { get; set; }
        public string StationName { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ===== MAINTENANCE RECORDS =====
    public class MaintenanceRecord
    {
        public int MaintenanceId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleLabel { get; set; }   // joined
        public int ServiceTypeId { get; set; }
        public string ServiceName { get; set; }     // joined
        public DateTime MaintenanceDate { get; set; }
        public decimal Cost { get; set; }
        public long? OdometerReading { get; set; }
        public string Description { get; set; }
        public string GarageName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ===== REMINDERS =====
    public class Reminder
    {
        public int ReminderId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleLabel { get; set; }    // joined
        public int? ServiceTypeId { get; set; }
        public string ServiceName { get; set; }      // joined
        public DateTime ReminderDate { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }   // Pending / Completed / Cancelled
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public int DaysLeft
        {
            get { return (int)(ReminderDate.Date - DateTime.Now.Date).TotalDays; }
        }
    }
}
