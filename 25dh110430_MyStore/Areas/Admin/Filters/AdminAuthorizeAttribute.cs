using System.Linq;
using System.Web;
using System.Web.Mvc;
using _25dh110430_MyStore.Models;

namespace _25dh110430_MyStore.Areas.Admin.Filters
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null ||
                httpContext.User == null ||
                !httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            string username = httpContext.User.Identity.Name;
            

            if (string.IsNullOrWhiteSpace(username))
                return false;

            using (var db = new MyStoreEntities())
            {
                var user = db.Users.FirstOrDefault(
                    u => u.Username == username
                );

                if (user == null)
                    return false;

                return user.UserRole != null &&
                       user.UserRole.Trim() == "A";
            }
        }

        protected override void HandleUnauthorizedRequest(
            AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" },
                        { "area", "" }
                    }
                );
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "Index" },
                        { "area", "" }
                    }
                );
            }
        }
    }
}