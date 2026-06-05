using System;
using System.Web;
using System.Web.Mvc;
using CarCareTracker.Models;
using CarCareTracker.Models.ViewModels;
using CarCareTracker.Services;

namespace CarCareTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _auth = new AuthService();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            User user;
            var token = _auth.Login(model, out user);

            if (token == null)
            {
                ModelState.AddModelError("", "Invalid email or password, or account is inactive.");
                return View(model);
            }

            // Store JWT in an HttpOnly cookie
            var cookie = new HttpCookie("jwt_token", token)
            {
                HttpOnly = true,
                Expires = model.RememberMe ? DateTime.Now.AddDays(7) : DateTime.Now.AddHours(2)
            };
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var error = _auth.Register(model);
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(model);
            }

            TempData["Success"] = "Account created successfully. Please log in.";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["jwt_token"] != null)
            {
                var cookie = new HttpCookie("jwt_token") { Expires = DateTime.Now.AddDays(-1) };
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Login");
        }
    }
}
