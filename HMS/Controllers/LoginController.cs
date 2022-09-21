using System;
using System.Security.Claims;
using WebHttp = System.Web.Http;
using System.Web.Mvc;
using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using HMS.DataAccess.UnitOfwork;
using HMS.DataAccess.Service;
using System.Linq;
using System.Web.Routing;
using System.Reflection;
using HMS.Models;

namespace HMS.Controllers
{
    [RoutePrefix("Login")]
    public class LoginController : Controller
    {
        [HttpGet, AllowAnonymous, Route()]
        public ActionResult Index(Status Model)
        {

            if (Session["User"] != null)
                return RedirectToAction("Index", "Home");
            else
                return View(Model);
        }
        [HttpGet, AllowAnonymous, Route("{UserName}/{Password}")]
        public RedirectToRouteResult Login(string UserName, string Password)
        {
            UnitOfWork dbContext = new UnitOfWork(new ConnectionFactory());
            try
            {
                var Model = new User
                {
                    UserName = UserName,
                    Password = Password,
                    Terminal = Request.UserHostName
                };
                if (string.IsNullOrEmpty(Model.UserName) || string.IsNullOrEmpty(Model.Password))
                {
                    return RedirectToAction("Index", "Login", new Status
                    {
                        Message = "UserName/Password can't be empty"
                    });
                }
                if (dbContext.UserRepo.IsValidUser(Model))
                {
                    Session["User"] = Model;
                    var Identity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Model.UserName) }, "ApplicationCookie");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Login", new Status
                    {
                        Message = "Invalid UserName of Password, login denied"
                    });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login", new Status
                {
                    Message = ex.Message
                });
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        [HttpGet, AllowAnonymous, Route("QuickLogin")]
        public RedirectToRouteResult Login([WebHttp.FromUri]QuickLogin Entity)
        {
            UnitOfWork dbContext = new UnitOfWork(new ConnectionFactory());
            try
            {
                var RouteParam = Entity.Route?.Split(new char[] { '|' });
                var Model = new User
                {
                    UserName = Entity.UserName,
                    Password = Entity.Password,
                    Terminal = Request.UserHostName
                };
                if (string.IsNullOrEmpty(Model.UserName) || string.IsNullOrEmpty(Model.Password))
                {
                    return RedirectToAction("Index", "Login", new Status
                    {
                        Message = "UserName/Password can't be empty"
                    });
                }
                if (dbContext.UserRepo.IsValidUser(Model))
                {
                    Session["User"] = Model;
                    var Identity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Model.UserName) }, "ApplicationCookie");
                    if (string.IsNullOrEmpty(Entity.Route) || RouteParam.Length != 2)
                        return RedirectToAction("MRD", "Home");
                    else
                        return RedirectToAction(RouteParam[1], RouteParam[0]);
                }
                else
                {
                    return RedirectToAction("Index", "Login", new Status
                    {
                        Message = "Invalid UserName of Password, login denied"
                    });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login", new Status
                {
                    Message = ex.Message
                });
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        [HttpGet, AllowAnonymous, Route("QuickLoginV2")]
        public RedirectToRouteResult LoginV2([WebHttp.FromUri]QuickLogin Entity)
        {
            UnitOfWork dbContext = new UnitOfWork(new ConnectionFactory());
            try
            {
                var form = new MenuDetail();
                var Model = new User
                {
                    UserName = Entity.UserName.Base64Decode(),
                    Password = Entity.Password.Base64Decode(),
                    Terminal = Request.UserHostName
                };
                if (string.IsNullOrEmpty(Model.UserName) || string.IsNullOrEmpty(Model.Password))
                {
                    return RedirectToAction("Index", "Login", new Status
                    {
                        Message = "UserName/Password can't be empty"
                    });
                }
                if (dbContext.UserRepo.IsValidUser(Model))
                {
                    Session["User"] = Model;
                    var RouteValue = Entity.GetRouteValue();
                    var Identity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Model.UserName) }, "ApplicationCookie");
                    if (form != null)
                        return RedirectToAction(form.Action, form.Controller, RouteValue);
                    else
                        return RedirectToAction("PageNotFound", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Login", new Status
                    {
                        Message = "Invalid UserName of Password, login denied"
                    });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("PageNotFound", "Home");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpPost, AllowAnonymous]
        public RedirectToRouteResult LoginPost(User Model)
        {
            UnitOfWork dbContext = new UnitOfWork(new ConnectionFactory());
            try
            {
                if (string.IsNullOrEmpty(Model.UserName) || string.IsNullOrEmpty(Model.Password))
                {
                    return RedirectToAction("Index", "Login", new Status
                    {
                        Message = "UserName/Password can't be empty"
                    });
                }
                if (dbContext.UserRepo.IsValidUser(Model))
                {
                    Model.Terminal = Request.UserHostName;
                    Session["User"] = Model;
                    var Identity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Model.UserName) }, "ApplicationCookie");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Login", new Status
                    {
                        Message = "Invalid UserName of Password, login denied"
                    });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login", new Status
                {
                    Message = ex.Message
                });
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        [HttpGet]
        public RedirectToRouteResult LogOut()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Login");
        }

    }
    public class QuickLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Route { get; set; }
        public string UHID { get; set; }
        public string CompanyID { get; set; }
        public string EmpCode { get; set; }
        public string BatchNo { get; set; }
        public string FormatTestName { get; set; }
        public string Cntype { get; set; }
        public string IPDNo { get; set; }
    }
    public static class QuickLoginExtension
    {
        public static RouteValueDictionary GetRouteValue(this QuickLogin source)
        {
            var Route = new RouteValueDictionary();
            foreach (PropertyInfo item in source.GetType().GetProperties().Where(x => x.Name.NotIn("UserName", "Password", "Route")))
            {
                if (item.GetValue(source) != null)
                    Route.Add(item.Name, item.GetValue(source));
            }
            return Route;
        }
    }
}