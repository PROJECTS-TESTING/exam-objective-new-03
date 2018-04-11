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
    public class ContentsController : Controller
    {
        // GET: Contents
        public ActionResult Index()
        {
            var user = Session["User"] as UserSystemModel;
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (user.Status == "student")
            {
                return RedirectToAction("Index", "Student");
            }
            using (var DB = new dbEntities())
            {
                var SubjectData = (from s in DB.Subjects
                                   join u in DB.UserSystem on s.UserID equals u.UserID
                                   where u.UserID == user.UserID
                                   orderby s.SubjectID
                                   select new SubjectsModel
                                   {
                                       SubjectID = s.SubjectID,
                                       SubjectName = s.SubjectName,
                                       UserID = s.UserID
                                   }).ToList();
                ViewBag.SubjectContent = SubjectData;
            }
            return View();
        }

        // เพิ่มเนื้อหา
        public ActionResult Addcontent(string SubjecID)
        {
            var user = Session["User"] as UserSystemModel;
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (user.Status == "student")
            {
                return RedirectToAction("Index", "Student");
            }
            using (var DB = new dbEntities())
            {
                var SubjectData = (from s in DB.Subjects
                                   join u in DB.UserSystem on s.UserID equals u.UserID
                                   where u.UserID == user.UserID && s.SubjectID == SubjecID
                                   orderby s.SubjectID
                                   select new SubjectsModel
                                   {
                                       SubjectID = s.SubjectID,
                                       SubjectName = s.SubjectName,
                                       UserID = s.UserID
                                   }).ToList();
                ViewBag.Subject = SubjectData;
            }
            using (var DB = new dbEntities())
            {
                var SubjectShare = (from s in DB.Subjects
                                    join p in DB.Participant on s.SubjectID equals p.SubjectID
                                    where p.ParticipantID == user.UserID && p.SubjectID == SubjecID
                                    orderby p.SubjectID
                                    select new SubjectsModel
                                    {
                                        SubjectID = s.SubjectID,
                                        SubjectName = s.SubjectName,
                                        UserID = p.UserID
                                    }).ToList();
                ViewBag.SubjectShare = SubjectShare;
            }
            using (var DB = new dbEntities())
            {
                var LessonData = (from l in DB.Lesson
                                  where user.UserID == l.UserID && l.SubjectID == SubjecID
                                  orderby l.LessonID
                                  select new LessonModel
                                  {
                                      LessonID = l.LessonID,
                                      LesName = l.LesName,
                                      TextLesson = l.TextLesson

                                  }).DistinctBy(r => r.LesName).ToList();
                ViewBag.Lesson = LessonData;
                var ObjectiveDatanotDis = (from o in DB.Objective
                                           join l in DB.Lesson on o.LessonID equals l.LessonID
                                           let oPropos = (           //join และนับค่าที่ต้องการ
                                           from p in DB.Proposition
                                           where p.ObjID == o.ObjID
                                           select o).Count()
                                           where l.SubjectID == SubjecID
                                           select new ObjectiveModel
                                           {
                                               ObjID = o.ObjID,
                                               ObjName = o.ObjName,
                                               TextObj = o.TextObj,
                                               PLessonID = o.LessonID,
                                               CountProposID = oPropos
                                           }).ToList();
                ViewBag.ObjectiveNotDis = ObjectiveDatanotDis;
            }
            using (var DB = new dbEntities())
            {
                var UserIDData = (from u in DB.UserSystem
                                  where u.UserID == user.UserID
                                  orderby u.UserID
                                  select new UserSystemModel
                                  {
                                      UserID = u.UserID,
                                      Fname = u.Fname,
                                      Lname = u.Lname,
                                      Status = u.Status
                                  }).ToList();
                ViewBag.UserIDdata = UserIDData;
            }
            return View();
        }

               //// Update Lesson By LessonID
        public JsonResult EditLesson(string id)
        {
            var jsonreturn = new JsonRespone();
            int Id = int.Parse(id);
            try
            {
                using (var DB = new dbEntities())
                {
                    var EditLesson = (from le in DB.Lesson
                                      join su in DB.Subjects on le.SubjectID equals su.SubjectID
                                      where le.LessonID == Id
                                      select new LessonModel
                                      {
                                          LessonID = le.LessonID,
                                          LesName = le.LesName,
                                          TextLesson = le.TextLesson,
                                          SubjectID = le.SubjectID,
                                          SubjectName = su.SubjectName,
                                          UserID = le.UserID,
                                          Contents = le.Contents
                                      }).FirstOrDefault();
                    if (EditLesson == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย", data = EditLesson };
                    }

                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }

        public JsonResult CreateLesson(LessonModel lesson)
        {
            var jsonretern = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {

                        var lessonUpdate = DB.Lesson.Where(x => x.LessonID == lesson.LessonID).FirstOrDefault();
                        if (lessonUpdate != null)
                        {
                            DB.Lesson.Where(x => x.LessonID == lessonUpdate.LessonID).ForEach(x =>
                            {
                                x.LessonID = lesson.LessonID;
                                x.Contents = lesson.Contents;
                            });
                            DB.SaveChanges();
                        }
                    
                    jsonretern = new JsonRespone { status = true, message = "บันทึกเรียบร้อย" };
                }

            }
            catch (Exception ex)
            {
                jsonretern = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonretern);
        }
    }
}