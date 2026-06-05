using System;
using System.Web.Mvc;
using CarCareTracker.Helpers;
using CarCareTracker.Models;
using CarCareTracker.Repositories;

namespace CarCareTracker.Controllers
{
    [JwtAuthorize]
    public class VehiclesController : Controller
    {
        private readonly VehicleRepository _vehicles = new VehicleRepository();
        private readonly LookupRepository _lookup = new LookupRepository();

        public ActionResult Index()
        {
            SetUserViewBag();
            var list = _vehicles.GetByUser(this.CurrentUserId());
            return View(list);
        }

        public ActionResult Details(int id)
        {
            SetUserViewBag();
            var v = _vehicles.GetById(id);
            if (v == null || v.UserId != this.CurrentUserId()) return HttpNotFound();
            return View(v);
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetUserViewBag();
            LoadLookups();
            return View(new Vehicle { YearMade = DateTime.Now.Year });
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create(Vehicle vehicle)
        {
            SetUserViewBag();

            if (vehicle == null)
            {
                LoadLookups();
                ModelState.AddModelError("", "Form data was not received. Please try again.");
                return View(new Vehicle { YearMade = DateTime.Now.Year });
            }

            // Manual validation for required dropdown selection
            if (vehicle.TypeId <= 0)
            {
                ModelState.AddModelError("TypeId", "Please select a Vehicle Type.");
            }

            if (!ModelState.IsValid) { LoadLookups(); return View(vehicle); }

            try
            {
                vehicle.UserId = this.CurrentUserId();
                _vehicles.Create(vehicle);
                TempData["Success"] = "Vehicle added successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not save vehicle: " + ex.Message);
                LoadLookups();
                return View(vehicle);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            SetUserViewBag();
            var v = _vehicles.GetById(id);
            if (v == null || v.UserId != this.CurrentUserId()) return HttpNotFound();
            LoadLookups();
            return View(v);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Edit(Vehicle vehicle)
        {
            SetUserViewBag();

            if (vehicle.TypeId <= 0)
            {
                ModelState.AddModelError("TypeId", "Please select a Vehicle Type.");
            }

            if (!ModelState.IsValid) { LoadLookups(); return View(vehicle); }

            try
            {
                var existing = _vehicles.GetById(vehicle.VehicleId);
                if (existing == null || existing.UserId != this.CurrentUserId()) return HttpNotFound();

                _vehicles.Update(vehicle);
                TempData["Success"] = "Vehicle updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not update vehicle: " + ex.Message);
                LoadLookups();
                return View(vehicle);
            }
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var v = _vehicles.GetById(id);
            if (v == null || v.UserId != this.CurrentUserId()) return HttpNotFound();
            _vehicles.Delete(id);
            TempData["Success"] = "Vehicle deleted successfully.";
            return RedirectToAction("Index");
        }

        private void LoadLookups()
        {
            ViewBag.VehicleTypes = _lookup.GetVehicleTypes();
        }

        private void SetUserViewBag()
        {
            ViewBag.UserName = this.CurrentUserName();
            ViewBag.UserRole = this.CurrentUserRole();
        }
    }
}
