using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarCareTracker.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalVehicles { get; set; }
        public int ActiveVehicles { get; set; }
        public decimal FuelExpensesThisMonth { get; set; }
        public decimal MaintenanceCostThisMonth { get; set; }
        public int UpcomingRemindersCount { get; set; }

        public List<Reminder> UpcomingServices { get; set; }
        public List<ActivityItem> RecentActivity { get; set; }

        // For chart (last 6 months)
        public List<string> ChartMonths { get; set; }
        public List<decimal> ChartFuel { get; set; }
        public List<decimal> ChartMaintenance { get; set; }

        public DashboardViewModel()
        {
            UpcomingServices = new List<Reminder>();
            RecentActivity = new List<ActivityItem>();
            ChartMonths = new List<string>();
            ChartFuel = new List<decimal>();
            ChartMaintenance = new List<decimal>();
        }
    }

    public class ActivityItem
    {
        public string Description { get; set; }
        public DateTime When { get; set; }
        public string Icon { get; set; }   // fuel / tool / bell / car
    }

    public class ReportsViewModel
    {
        public decimal TotalFuelCost { get; set; }
        public decimal TotalMaintenanceCost { get; set; }
        public int TotalRecords { get; set; }
        public decimal AverageMonthly { get; set; }

        // Computed using the Strategy design pattern (FuelEfficiencyCalculator)
        public string FuelEfficiency { get; set; }

        public List<string> ChartMonths { get; set; }
        public List<decimal> ChartFuel { get; set; }
        public List<decimal> ChartMaintenance { get; set; }

        public List<VehicleExpenseRow> Breakdown { get; set; }

        public ReportsViewModel()
        {
            ChartMonths = new List<string>();
            ChartFuel = new List<decimal>();
            ChartMaintenance = new List<decimal>();
            Breakdown = new List<VehicleExpenseRow>();
        }
    }

    public class VehicleExpenseRow
    {
        public string VehicleLabel { get; set; }
        public decimal FuelCost { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal TotalExpense { get { return FuelCost + MaintenanceCost; } }
        public int Records { get; set; }
        public DateTime? LastActivity { get; set; }
    }
}
