using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Routing;
using CarCareTracker.Services;

namespace CarCareTracker.Helpers
{
    /// <summary>
    /// Action filter that reads the JWT from a cookie, validates it,
    /// and sets the current user principal. Redirects to Login if invalid,
    /// or to Forbidden (403) if the role is not allowed.
    /// </summary>
    public class JwtAuthorizeAttribute : ActionFilterAttribute
    {
        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["jwt_token"];
            var token = cookie != null ? cookie.Value : null;

            var jwt = new JwtService();
            var principal = jwt.ValidateToken(token);

            if (principal == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                var roleClaim = principal.FindFirst(ClaimTypes.Role) != null
                    ? principal.FindFirst(ClaimTypes.Role).Value : "";
                bool ok = Roles.Split(',').Any(r => string.Equals(r.Trim(), roleClaim, StringComparison.OrdinalIgnoreCase));
                if (!ok)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Error", action = "Forbidden" }));
                    return;
                }
            }

            filterContext.HttpContext.User = principal;
            base.OnActionExecuting(filterContext);
        }
    }

    /// <summary>Extension helpers to read claims from the current logged-in user.</summary>
    public static class ControllerUserExtensions
    {
        public static int CurrentUserId(this Controller c)
        {
            var p = c.User as ClaimsPrincipal;
            var id = p != null && p.FindFirst(ClaimTypes.NameIdentifier) != null
                ? p.FindFirst(ClaimTypes.NameIdentifier).Value : null;
            return string.IsNullOrEmpty(id) ? 0 : int.Parse(id);
        }

        public static string CurrentUserName(this Controller c)
        {
            var p = c.User as ClaimsPrincipal;
            return p != null && p.FindFirst(ClaimTypes.Name) != null
                ? p.FindFirst(ClaimTypes.Name).Value : "User";
        }

        public static string CurrentUserRole(this Controller c)
        {
            var p = c.User as ClaimsPrincipal;
            return p != null && p.FindFirst(ClaimTypes.Role) != null
                ? p.FindFirst(ClaimTypes.Role).Value : "User";
        }
    }
}
