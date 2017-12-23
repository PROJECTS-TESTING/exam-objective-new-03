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
                               where m.UserID == user.UserID && g.SubjectID == etid && g.UserID == sid select new{g.GroupID, g.GroupName,g.GroupPW}).ToList();
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
                   
                    return RedirectToAction("Subjecttest", "Student",new { subid = etid, datasu = sid, g = idGroup[0].GroupID });
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

        public ActionResult Subjecttest(string subid, string datasu, int g)
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
            using (var DB = new dbEntities())
            {
                ViewBag.DataExamtopic = (from e in DB.ExamTopic
                                         where e.GroupID == g && e.SubjectID == subid && e.UserID == datasu
                                         select new ExamTopicModel
                                         {
                                             ExamtopicID = e.ExamtopicID,
                                             ExamtopicName = e.ExamtopicName,
                                             DatetoBegin = e.DatetoBegin,
                                             TimetoBegin = e.TimetoBegin,
                                             TimetoEnd = e.TimetoEnd,
                                             NewPage = e.NewPage,
                                             HowtoPage = e.HowtoPage,
                                             ExamtopicPW = e.ExamtopicPW

                                         }).ToList();
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

        public JsonResult ViewTesting(ExamtopicDataModel DataEx)
        {
            var jsonretern = new JsonRespone();
            
            using (var DB = new dbEntities())
            {
                var examtopid = (from e in DB.ExamTopic
                                 where e.ExamtopicID == DataEx.ExamtopicID
                                 select e.ExamtopicPW).FirstOrDefault();
                var dataexamtopic = new ExamtopicDataModel {ExamtopicID = DataEx.ExamtopicID,SubjectID = DataEx.SubjectID, UserID = DataEx.UserID };
                if (examtopid == null && DataEx.CheckDateTime == 0)
                { 
                    jsonretern = new JsonRespone { status = true, message = "เข้าสอบเรียบร้อย", data = dataexamtopic };
                }
                else if(examtopid != null && examtopid == DataEx.ExamtopicPW && DataEx.CheckDateTime == 0)
                {
                    jsonretern = new JsonRespone { status = true, message = "เข้าสอบเรียบร้อย", data = dataexamtopic };
                }else if (DataEx.CheckDateTime == 1)
                {
                    jsonretern = new JsonRespone { status = false, message = "เวลาในการทำแบบทดสอบหมดไปแล้ว" };
                }
                else if (DataEx.CheckDateTime == 2)
                {
                    jsonretern = new JsonRespone { status = false, message = "ยังไม่ถึงเวลาในการทำแบบทดสอบ" };
                }
                else
                {
                    jsonretern = new JsonRespone { status = false, message = "รหัสเข้าสอบไม่ถูกต้อง" };
                }
            }
            
            return Json(jsonretern);
        }
        public ActionResult ViewTestings(string subid, string datasu, int e)
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
                ViewBag.DataExamtopic = (from e1 in DB.ExamTopic
                                         where e1.ExamtopicID == e
                                         select new ExamTopicModel
                                         {
                                             ExamtopicID = e1.ExamtopicID,
                                             ExamtopicName = e1.ExamtopicName,
                                             DatetoBegin = e1.DatetoBegin,
                                             TimetoBegin = e1.TimetoBegin,
                                             TimetoEnd = e1.TimetoEnd,
                                             NewPage = e1.NewPage,
                                             HowtoPage = e1.HowtoPage,

                                         }).ToList();
            }
            using (var DB = new dbEntities())
            {
                ViewBag.dataGroup = (from gg in DB.TestGroup
                                     where gg.GroupID == (from ee in DB.ExamTopic where ee.ExamtopicID == e select ee.GroupID).FirstOrDefault()
                                     select gg.GroupName).FirstOrDefault();
            }
            using (var DB = new dbEntities())
            {
                ViewBag.dataSubject = (from s in DB.Subjects where s.SubjectID == subid && s.UserID == datasu select s.SubjectName).FirstOrDefault();
            }
            using (var DB = new dbEntities())
            {
                var dataExambody = (from ee in DB.ExamBody where ee.ExamtopicID == e select ee.ExamBodyID).FirstOrDefault();
                ViewBag.dataProposition = (from ex in DB.GetExam
                                           join p in DB.Proposition on ex.ProposID equals p.ProposID
                                           where ex.ExamBodyID == dataExambody
                                           select new PropositionModel
                                           {
                                               ProposID = ex.ProposID,
                                               ProposName = p.ProposName,
                                               TextPropos = p.TextPropos
                                           }).ToList();
            }
            using (var DB = new dbEntities())
            {
                var dataExambody = (from ee in DB.ExamBody where ee.ExamtopicID == e select ee.ExamBodyID).FirstOrDefault();
                ViewBag.dataChoice = (from ex in DB.GetExam
                                      join c in DB.Choice on ex.ProposID equals c.ProposID
                                      where ex.ExamBodyID == dataExambody
                                      select new ChoiceModel
                                      {
                                          ProposID = ex.ProposID,
                                          ChoiceID = c.ChoiceID,
                                          TextChoice = c.TextChoice
                                      }).ToList();
            }
            return View();
        }
    }
}