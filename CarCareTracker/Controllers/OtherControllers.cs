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
    public class ReportsController : Controller
    {
        private readonly VehicleRepository _vehicles = new VehicleRepository();
        private readonly FuelRepository _fuel = new FuelRepository();
        private readonly MaintenanceRepository _maint = new MaintenanceRepository();

        public ActionResult Index()
        {
            ViewBag.UserName = this.CurrentUserName();
            ViewBag.UserRole = this.CurrentUserRole();

            int uid = this.CurrentUserId();
            var vehicles = _vehicles.GetByUser(uid);
            var fuel = _fuel.GetByUser(uid);
            var maint = _maint.GetByUser(uid);

            var vm = new ReportsViewModel
            {
                TotalFuelCost = fuel.Sum(f => f.Cost),
                TotalMaintenanceCost = maint.Sum(m => m.Cost),
                TotalRecords = fuel.Count + maint.Count
            };
            vm.AverageMonthly = vm.TotalRecords > 0 ? (vm.TotalFuelCost + vm.TotalMaintenanceCost) / 6m : 0;

            // ===== STRATEGY PATTERN in action =====
            // Compute fuel efficiency using an interchangeable algorithm.
            var calculator = new CarCareTracker.Patterns.Strategy.FuelEfficiencyCalculator(
                new CarCareTracker.Patterns.Strategy.KmPerLiterStrategy());
            double eff = calculator.Compute(fuel);
            vm.FuelEfficiency = eff > 0 ? calculator.ResultLabel(eff) : "Not enough data";

            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i);
                var mStart = new DateTime(month.Year, month.Month, 1);
                var mEnd = mStart.AddMonths(1);
                vm.ChartMonths.Add(month.ToString("MMM"));
                vm.ChartFuel.Add(fuel.Where(f => f.FuelDate >= mStart && f.FuelDate < mEnd).Sum(f => f.Cost));
                vm.ChartMaintenance.Add(maint.Where(m => m.MaintenanceDate >= mStart && m.MaintenanceDate < mEnd).Sum(m => m.Cost));
            }

            foreach (var v in vehicles)
            {
                var label = v.Brand + " " + v.Model;
                var vfuel = fuel.Where(f => f.VehicleId == v.VehicleId).ToList();
                var vmaint = maint.Where(m => m.VehicleId == v.VehicleId).ToList();
                var dates = vfuel.Select(f => f.FuelDate).Concat(vmaint.Select(m => m.MaintenanceDate)).ToList();

                vm.Breakdown.Add(new VehicleExpenseRow
                {
                    VehicleLabel = label,
                    FuelCost = vfuel.Sum(f => f.Cost),
                    MaintenanceCost = vmaint.Sum(m => m.Cost),
                    Records = vfuel.Count + vmaint.Count,
                    LastActivity = dates.Any() ? dates.Max() : (DateTime?)null
                });
            }

            return View(vm);
        }
    }

    [JwtAuthorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserRepository _users = new UserRepository();

        public ActionResult Users()
        {
            ViewBag.UserName = this.CurrentUserName();
            ViewBag.UserRole = this.CurrentUserRole();

            var list = _users.GetAll();
            ViewBag.TotalUsers = list.Count;
            ViewBag.ActiveUsers = list.Count(u => u.IsActive);
            ViewBag.AdminAccounts = list.Count(u => u.RoleName == "Admin");
            ViewBag.NewThisMonth = list.Count(u => u.CreatedAt >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            return View(list);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(int id, bool active)
        {
            _users.SetActive(id, active);
            TempData["Success"] = "User status updated.";
            return RedirectToAction("Users");
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (id == this.CurrentUserId())
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction("Users");
            }
            _users.Delete(id);
            TempData["Success"] = "User deleted.";
            return RedirectToAction("Users");
        }
    }

    public class ErrorController : Controller
    {
        public ActionResult Forbidden()
        {
            Response.StatusCode = 403;
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}
