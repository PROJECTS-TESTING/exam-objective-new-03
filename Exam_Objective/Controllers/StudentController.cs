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
using ConsoleAppLog;
using Networks;

namespace Exam_Objective.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            var user = Session["User"] as UserSystemModel;
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (user.Status != "student")
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        public ActionResult Subjects()
        {
            var user = Session["User"] as UserSystemModel;
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (user.Status != "student")
            {
                return RedirectToAction("Login", "Login");
            }
           
            using (var DB = new dbEntities())
            {
                var SubjectData = (from s in DB.Subjects
                                   join u in DB.UserSystem on s.UserID equals u.UserID
                                   orderby s.SubjectID
                                   select new SubjectsModel
                                   {
                                       SubjectID = s.SubjectID,
                                       SubjectName = s.SubjectName,
                                       UserID = s.UserID,
                                       Fname = u.Fname,
                                       Lname = u.Lname
                                   }).ToList();
                ViewBag.Subject = SubjectData;
            }
            return View();
        }

        public ActionResult SubjectLogin(string etid, string sid)
        {
            var user = Session["User"] as UserSystemModel;
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (user.Status != "student")
            {
                return RedirectToAction("Login", "Login");
            }

            using (var DB = new dbEntities())
            {
                var idGroup = (from m in DB.Member
                               join g in DB.TestGroup on m.GroupID equals g.GroupID
                               where m.UserID == user.UserID && g.SubjectID == etid && g.UserID == sid select new{ g.GroupName,g.GroupPW}).ToList();
                var SubjectData = (from s in DB.Subjects
                                   join u in DB.UserSystem on s.UserID equals u.UserID
                                   where s.SubjectID == etid && u.UserID == sid
                                   select new SubjectsModel
                                   {
                                       SubjectID = s.SubjectID,
                                       SubjectName = s.SubjectName,
                                       UserID = s.UserID,
                                       Fname = u.Fname,
                                       Lname = u.Lname
                                   }).ToList();
                ViewBag.DataSubject = SubjectData;

                if (idGroup.Count !=0 && idGroup != null)
                {
                   
                    return RedirectToAction("Subjecttest", "Student",new { subid = etid, datasu = sid });
                }
                else
                {
                   
                }
            }

            return View();
        }

        public JsonResult AddLoginGroup(AddGroupStudent addG)
        {
            var jsonretern = new JsonRespone();
            var user = Session["User"] as UserSystemModel;
            try
            {
                using (var DB= new dbEntities())
                {
                    var dataGID = DB.TestGroup.Where(g => g.SubjectID == addG.SubjectID
                    && g.UserID == addG.UserID
                    && g.GroupName == addG.StudyGroup
                    && g.GroupPW == addG.GroupPW).FirstOrDefault();

                    if(dataGID != null) {
                        DB.Member.Add(new Models.Member
                        {
                            GroupID = dataGID.GroupID,
                            UserID  = user.UserID
                        });
                        DB.SaveChanges();
                        jsonretern = new JsonRespone { status = true, message = "บันทึกเรียบร้อย" };
                    }
                    else { jsonretern = new JsonRespone { status = true, message = "ข้อมูลที่ท่านกรอกยังไม่มีในระบบ" }; }
                   
                }
            }
            catch (Exception ex)
            {
                jsonretern = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonretern);
        }

        public ActionResult Subjecttest(string subid, string datasu)
        {
            var user = Session["User"] as UserSystemModel;
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (user.Status != "student")
            {
                return RedirectToAction("Login", "Login");
            }
            using (var DB = new dbEntities())
            {
                var SubjectData = (from s in DB.Subjects
                                   join u in DB.UserSystem on s.UserID equals u.UserID
                                   where s.SubjectID == subid && u.UserID == datasu
                                   select new SubjectsModel
                                   {
                                       SubjectID = s.SubjectID,
                                       SubjectName = s.SubjectName,
                                       UserID = s.UserID,
                                       Fname = u.Fname,
                                       Lname = u.Lname
                                   }).ToList();
                ViewBag.DataSubject = SubjectData;
            }
                return View();
        }
        public ActionResult MySubject()
        {
            var user = Session["User"] as UserSystemModel;
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (user.Status != "student")
            {
                return RedirectToAction("Login", "Login");
            }
            using(var DB = new dbEntities())
            {
                var subjectdata = (from m in DB.Member
                                   join g in DB.TestGroup 
                                   on m.GroupID equals g.GroupID
                                   join s in DB.Subjects 
                                   on g.SubjectID equals s.SubjectID 
                                   join u in DB.UserSystem
                                   on s.UserID equals u.UserID
                                   where m.UserID == user.UserID && g.SubjectID == s.SubjectID && g.UserID == s.UserID && s.UserID == u.UserID
                                   select new MySubjectsModel {
                                      GroupID = m.GroupID,
                                      SubjectID = g.SubjectID,
                                      UserID = g.UserID,
                                      SubjectName = s.SubjectName,
                                     Fname =  u.Fname,
                                      Lname = u.Lname }
                                   ).ToList();
                ViewBag.MySubject = subjectdata;
            }
            return View();
        }
    }
}