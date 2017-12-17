using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Net.Mime;
using System.IO;
using System.Text;
using Exam_Objective.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;

namespace Exam_Objective.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        public JsonResult GetLogin(UserSystemModel user)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var getLogin = (from u in DB.UserSystem
                                    where u.UserID == user.UserID & u.PWLogin == user.PWLogin
                                    select new UserSystemModel
                                    {
                                        UserID = u.UserID,
                                        PWLogin = u.PWLogin,
                                        Fname = u.Fname,
                                        Lname = u.Lname,
                                        Status = u.Status
                                    }).FirstOrDefault();
                    if (getLogin == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "username or password incorrect !" };

                    }
                    else
                    {
                        jsonreturn = new JsonRespone { status = true, message = "พบ", data = getLogin };
                        Session["User"] = new UserSystemModel() { UserID = getLogin.UserID, PWLogin = getLogin.PWLogin, Fname = getLogin.Fname, Lname = getLogin.Lname, Status = getLogin.Status, check = 1 };

                    }
                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เเกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
            
        }
        public ActionResult Logout()
        {
            //Session.Clear(); or Session[LogedStudents] = null;
            Session["User"] = new UserSystemModel() { UserID = null, PWLogin = null, Fname = null, Lname = null, Status = null };
            // Code disables caching by browser.
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("Login", "Login");
        }
    }
}