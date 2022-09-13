using HMS.DataAccess.Entity;
using HMS.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMS.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var user = Session["User"] as User;
            ViewBag.WelcomeUser = $"Welcome, {user.UserName.ToUpper()}";
            ViewBag.User = $"{user.UserName.ToUpper()}";

            return View();
        }

        #region Form
        public ActionResult Yojana()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var user = Session["User"] as User;
            ViewBag.WelcomeUser = $"Welcome, {user.UserName.ToUpper()}";
            return View();
        }
        [BrowserSupport]
        public ActionResult MRD()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var user = Session["User"] as User;
            ViewBag.WelcomeUser = $"Welcome, {user.UserName.ToUpper()}";
            return View();
        }
        #endregion

        #region Pages
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Unauthorized()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var user = Session["User"] as User;
            ViewBag.WelcomeUser = $"Welcome, {user.UserName.ToUpper()}";
            return View();
        }

        public ActionResult UnderConstruction()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var user = Session["User"] as User;
            ViewBag.WelcomeUser = $"Welcome, {user.UserName.ToUpper()}";
            return View();
        }

        public ActionResult PageNotFound()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var user = Session["User"] as User;
            ViewBag.WelcomeUser = $"Welcome, {user.UserName.ToUpper()}";
            return View();
        }

        public ActionResult BrowserSupport()
        {
            return View();
        }
        #endregion

    }
}