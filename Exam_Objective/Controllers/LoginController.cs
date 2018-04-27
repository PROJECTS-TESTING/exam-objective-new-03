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
using OneLogin.Saml;
using System.Xml;

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

        /**** BEGIN SSO ****/
        public ActionResult Index()
        {
            return RedirectToAction("Sso", "Login");
        }
        // ล็อกอิน
        public ActionResult Sso()
        {
            AppSettings appSettings = new AppSettings();
            Auth auth = new Auth(appSettings);
            string redirect = "";
            if (Request.QueryString["redirect"] != null)
            {
                redirect = Request.QueryString["redirect"];
            }
            else if (Request.UrlReferrer != null)
            {
                redirect = Request.UrlReferrer.ToString();
            }
            if (redirect != "")
            {
                auth.Login(redirect);
            }
            else
            {
                auth.Login();
            }
            return new EmptyResult();
        }
        // ล็อกอินผ่าน
        public ActionResult Acs()
        {
            AppSettings appSettings = new AppSettings();
            Auth auth = new Auth(appSettings);
            auth.ProcessResponse();
            var res = string.Empty;
            var name = string.Empty;
            var ssoValue = string.Empty;
            var modelSso = new ModelSSO();
            if (auth.Response.IsValid())
            {

                HttpContext.Session["ssoNameID"] = auth.Response.GetNameID();

                HttpContext.Session["ssoSessionIndex"] = auth.Response.GetSessionIndex();

                HttpContext.Session["ssoUserData"] = auth.Response.GetAttributes();

                if (Request.Form["RelayState"] != null)
                {
                    res = Request.Form["RelayState"].ToString();
                }
                if (HttpContext.Session["ssoUserData"] != null)
                {
                    XmlDocument userXmlDoc = new XmlDocument();
                    userXmlDoc.PreserveWhitespace = true;
                    userXmlDoc.XmlResolver = null;
                    userXmlDoc.LoadXml((string)HttpContext.Session["ssoUserData"]);

                    var uid = "";
                    var firstNameThai = "";
                    var lastNameThai = "";
                    var studentId = "";
                    var status = "";
                    var prename = "";
                    var gidNumber = "";
                    foreach (XmlNode node in userXmlDoc.FirstChild.ChildNodes)
                    {
                        name = node.Attributes["Name"].Value;
                        ssoValue = node.FirstChild.InnerText;
                        switch (name)
                        {
                            case "uid":
                                uid = ssoValue;
                                break;
                            case "firstNameThai":
                                firstNameThai = ssoValue;
                                break;
                            case "lastNameThai":
                                lastNameThai = ssoValue;
                                break;
                            case "prename":
                                prename = ssoValue;
                                break;
                            case "studentId":
                                studentId = ssoValue;
                                break;
                            case "gidNumber":
                                gidNumber = ssoValue;
                                break;
                            default:
                                break;
                        }
                    }
                    if (gidNumber == "4500")
                    {
                        status = "student";
                    }
                    else if (gidNumber == "2500")
                    {
                        prename = "อาจารย์";
                        status = "professor";
                    }
                    else if (gidNumber == "3000")
                    {
                        status = "admin";
                    }
                    HttpContext.Session["User"] = new UserSystemModel() { UserID = uid, StudentID = studentId, Fname = prename + firstNameThai, Lname = lastNameThai, Status = status };
                    var user = HttpContext.Session["User"] as UserSystemModel;
                    try
                    {
                        using (var DB = new dbEntities())
                        {
                            var cUser = DB.UserSystem.Where(r => r.UserID == user.UserID).FirstOrDefault();
                            if (cUser == null)
                            {
                                UserSystem u = new UserSystem();
                                u.UserID = user.UserID;
                                u.Fname = user.Fname;
                                u.Lname = user.Lname;
                                u.StudentID = user.StudentID;
                                u.Status = user.Status;                                
                                u.PWLogin = "12345678";
                                DB.UserSystem.Add(u);
                                DB.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    if (user.Status == "student")
                    {
                        return RedirectToAction("Index", "Student");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // ไม่มีข้อมูลผู้ใช้งาน
                    return new EmptyResult();
                }
            }
            else
            {
                //ไม่มีการล็อกอิน
                return new EmptyResult();
            }
        }
        // ล็อกเอา
        public ActionResult Slo()
        {
            AppSettings appSettings = new AppSettings();
            Auth auth = new Auth(appSettings);
            HttpContext.Session["ssoUserData"] = null;
            HttpContext.Session["User"] = null; //Session ใน งาน
            string nameId = (string)HttpContext.Session["ssoNameID"];
            string sessionIndex = (string)HttpContext.Session["ssoSessionIndex"];

            string redirect = "";
            if (Request.QueryString["redirect"] != null)
            {
                redirect = Request.QueryString["redirect"];
            }
            else if (Request.UrlReferrer != null)
            {
                redirect = Request.UrlReferrer.ToString();
            }

            if (redirect != "")
            {
                auth.Logout(redirect, nameId, sessionIndex);
            }
            else
            {
                auth.Logout("", nameId, sessionIndex);
            }
            Response.End();
            return new EmptyResult();
        }
        // ล็อกเอาท์เสร็จแล้ว
        public ActionResult Sls()
        {
            AppSettings appSettings = new AppSettings();
            Auth auth = new Auth(appSettings);
            if (Request.Form["SAMLResponse"] != null)
            {
                auth.ProcessResponse();

                if (auth.Response.IsValid())
                {
                    HttpContext.Session["ssoNameID"] = null;
                    HttpContext.Session["ssoSessionIndex"] = null;
                    HttpContext.Session["User"] = null; //Session ในงาน
                    HttpContext.Session.Clear();
                    HttpContext.Session.RemoveAll();
                    HttpContext.Session.Abandon();
                    Session.RemoveAll();
                    Session.Clear();
                    Session.Abandon();
                    if (Request.Form["RelayState"] != null)
                    {
                        return Redirect(Request.Form["RelayState"]);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");// return RedirectToAction("Index");
                    }
                }
                else
                {
                    HttpContext.Session.Clear();
                    HttpContext.Session.RemoveAll();
                    HttpContext.Session.Abandon();
                    Session.RemoveAll();
                    Session.Clear();
                    Session.Abandon();
                    return RedirectToAction("Login", "Login");//return RedirectToAction("Index");
                }
            }
            else
            {
                HttpContext.Session.Clear();
                HttpContext.Session.RemoveAll();
                HttpContext.Session.Abandon();
                Session.RemoveAll();
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Login");//return RedirectToAction("Index");
            }
        }
        /**** END SSO ****/
    }
}