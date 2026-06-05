using System;
using System.Linq;
using System.Web.Mvc;
using CarCareTracker.Helpers;
using CarCareTracker.Models;
using CarCareTracker.Repositories;

namespace CarCareTracker.Controllers
{
    [JwtAuthorize]
    public class FuelController : Controller
    {
        private readonly FuelRepository _fuel = new FuelRepository();
        private readonly VehicleRepository _vehicles = new VehicleRepository();

        public ActionResult Index()
        {
            SetUserViewBag();
            return View(_fuel.GetByUser(this.CurrentUserId()));
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetUserViewBag();
            ViewBag.Vehicles = _vehicles.GetByUser(this.CurrentUserId());
            return View(new FuelRecord { FuelDate = DateTime.Now });
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create(FuelRecord model)
        {
            SetUserViewBag();
            if (!ModelState.IsValid)
            {
                ViewBag.Vehicles = _vehicles.GetByUser(this.CurrentUserId());
                return View(model);
            }
            _fuel.Create(model);
            TempData["Success"] = "Fuel record saved successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _fuel.Delete(id);
            TempData["Success"] = "Fuel record deleted.";
            return RedirectToAction("Index");
        }

        private void SetUserViewBag()
        {
            ViewBag.UserName = this.CurrentUserName();
            ViewBag.UserRole = this.CurrentUserRole();
        }
    }

    [JwtAuthorize]
    public class MaintenanceController : Controller
    {
        private readonly MaintenanceRepository _maint = new MaintenanceRepository();
        private readonly VehicleRepository _vehicles = new VehicleRepository();
        private readonly LookupRepository _lookup = new LookupRepository();

        public ActionResult Index()
        {
            SetUserViewBag();
            return View(_maint.GetByUser(this.CurrentUserId()));
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetUserViewBag();
            ViewBag.Vehicles = _vehicles.GetByUser(this.CurrentUserId());
            ViewBag.ServiceTypes = _lookup.GetServiceTypes();
            return View(new MaintenanceRecord { MaintenanceDate = DateTime.Now });
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create(MaintenanceRecord model)
        {
            SetUserViewBag();
            if (!ModelState.IsValid)
            {
                ViewBag.Vehicles = _vehicles.GetByUser(this.CurrentUserId());
                ViewBag.ServiceTypes = _lookup.GetServiceTypes();
                return View(model);
            }
            _maint.Create(model);
            TempData["Success"] = "Maintenance record saved successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _maint.Delete(id);
            TempData["Success"] = "Maintenance record deleted.";
            return RedirectToAction("Index");
        }

        private void SetUserViewBag()
        {
            ViewBag.UserName = this.CurrentUserName();
            ViewBag.UserRole = this.CurrentUserRole();
        }
    }

    [JwtAuthorize]
    public class RemindersController : Controller
    {
        private readonly ReminderRepository _reminders = new ReminderRepository();
        private readonly VehicleRepository _vehicles = new VehicleRepository();
        private readonly LookupRepository _lookup = new LookupRepository();

        public ActionResult Index()
        {
            SetUserViewBag();
            var list = _reminders.GetByUser(this.CurrentUserId());
            ViewBag.Total = list.Count;
            ViewBag.UpcomingWeek = list.Count(r => r.Status == "Pending" && r.DaysLeft >= 0 && r.DaysLeft <= 7);
            ViewBag.Overdue = list.Count(r => r.Status == "Pending" && r.DaysLeft < 0);
            ViewBag.Completed = list.Count(r => r.Status == "Completed");
            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetUserViewBag();
            ViewBag.Vehicles = _vehicles.GetByUser(this.CurrentUserId());
            ViewBag.ServiceTypes = _lookup.GetServiceTypes();
            return View(new Reminder { ReminderDate = DateTime.Now.AddDays(7), Status = "Pending" });
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create(Reminder model)
        {
            SetUserViewBag();
            if (!ModelState.IsValid)
            {
                ViewBag.Vehicles = _vehicles.GetByUser(this.CurrentUserId());
                ViewBag.ServiceTypes = _lookup.GetServiceTypes();
                return View(model);
            }
            _reminders.Create(model);
            TempData["Success"] = "Reminder created successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Complete(int id)
        {
            _reminders.UpdateStatus(id, "Completed");
            TempData["Success"] = "Reminder marked as completed.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _reminders.Delete(id);
            TempData["Success"] = "Reminder deleted.";
            return RedirectToAction("Index");
        }

        private void SetUserViewBag()
        {
            ViewBag.UserName = this.CurrentUserName();
            ViewBag.UserRole = this.CurrentUserRole();
        }
    }
}
