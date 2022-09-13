using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using HMS.DataAccess.UnitOfwork;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HMS.Helper
{
    public class CustomAuthorizationFilter : System.Web.Mvc.AuthorizeAttribute
    {
        private readonly string pageName;
        public CustomAuthorizationFilter(string PageName)
        {
            pageName = PageName;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorized = false;
            if (httpContext.Session["User"] == null)
                return false;

            var user = httpContext.Session["User"] as User;
            using (var DbContext = new UnitOfWork(new ConnectionFactory(Database.VEN), user))
            {
                authorized = DbContext.UserRepo.IsMenuAllowed(user, pageName?.ToUpper());
            }

            return authorized;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                {"controller","home" },
                {"action","Unauthorized" }
            });
        }
    }

    public class BrowserSupport : System.Web.Mvc.AuthorizeAttribute
    {
        public BrowserSupport()
        {

        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Request.Browser.Browser == "Chrome";

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                {"controller","home" },
                {"action","BrowserSupport" }
            });
        }
    }
}