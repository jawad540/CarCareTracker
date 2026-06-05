using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CarCareTracker.Helpers;
using CarCareTracker.Models.ViewModels;
using CarCareTracker.Repositories;

namespace CarCareTracker.Controllers
{
    [JwtAuthorize]
    public class DashboardController : Controller
    {
        private readonly VehicleRepository _vehicles = new VehicleRepository();
        private readonly FuelRepository _fuel = new FuelRepository();
        private readonly MaintenanceRepository _maint = new MaintenanceRepository();
        private readonly ReminderRepository _reminders = new ReminderRepository();

        public ActionResult Index()
        {
            int uid = this.CurrentUserId();
            ViewBag.UserName = this.CurrentUserName();
            ViewBag.UserRole = this.CurrentUserRole();

            var vehicles = _vehicles.GetByUser(uid);
            var fuel = _fuel.GetByUser(uid);
            var maint = _maint.GetByUser(uid);
            var upcoming = _reminders.GetUpcoming(uid, 30);

            // ===== OBSERVER PATTERN in action =====
            // When reminders are due, notify observers (dashboard alerts + log).
            var reminderSubject = new CarCareTracker.Patterns.Observer.ReminderSubject();
            var alertObserver = new CarCareTracker.Patterns.Observer.DashboardAlertObserver();
            var logObserver = new CarCareTracker.Patterns.Observer.LogObserver();
            reminderSubject.Subscribe(alertObserver);
            reminderSubject.Subscribe(logObserver);
            reminderSubject.CheckAndNotify(upcoming);
            ViewBag.ReminderAlerts = alertObserver.Alerts;

            var firstOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var vm = new DashboardViewModel
            {
                TotalVehicles = vehicles.Count,
                ActiveVehicles = vehicles.Count(v => v.IsActive),
                FuelExpensesThisMonth = fuel.Where(f => f.FuelDate >= firstOfMonth).Sum(f => f.Cost),
                MaintenanceCostThisMonth = maint.Where(m => m.MaintenanceDate >= firstOfMonth).Sum(m => m.Cost),
                UpcomingRemindersCount = upcoming.Count,
                UpcomingServices = upcoming.Take(5).ToList()
            };

            // Build 6-month chart data
            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i);
                var mStart = new DateTime(month.Year, month.Month, 1);
                var mEnd = mStart.AddMonths(1);
                vm.ChartMonths.Add(month.ToString("MMM"));
                vm.ChartFuel.Add(fuel.Where(f => f.FuelDate >= mStart && f.FuelDate < mEnd).Sum(f => f.Cost));
                vm.ChartMaintenance.Add(maint.Where(m => m.MaintenanceDate >= mStart && m.MaintenanceDate < mEnd).Sum(m => m.Cost));
            }

            // Recent activity (combine latest records)
            var activity = new List<ActivityItem>();
            foreach (var f in fuel.Take(3))
                activity.Add(new ActivityItem { Description = "Fuel record added for " + f.VehicleLabel, When = f.CreatedAt, Icon = "fuel" });
            foreach (var m in maint.Take(3))
                activity.Add(new ActivityItem { Description = "Maintenance record added for " + m.VehicleLabel, When = m.CreatedAt, Icon = "tool" });
            foreach (var v in vehicles.Take(2))
                activity.Add(new ActivityItem { Description = "Vehicle added: " + v.Brand + " " + v.Model, When = v.CreatedAt, Icon = "car" });

            vm.RecentActivity = activity.OrderByDescending(a => a.When).Take(5).ToList();

            return View(vm);
        }
    }
}
