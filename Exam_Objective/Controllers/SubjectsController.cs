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
    public class SubjectsController : Controller
    {
        // GET: Subjects
        public ActionResult Subjects()
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
                                       Theory = s.Theory,
                                       Practice = s.Practice,
                                       Course = s.Course,
                                       UserID = s.UserID
                                   }).ToList();
                ViewBag.Subject = SubjectData;
            }
            using (var DB = new dbEntities())
            {
                var SubjectShare = (from s in DB.Subjects
                                    join p in DB.Participant on s.SubjectID equals p.SubjectID
                                    where p.ParticipantID == user.UserID
                                    orderby s.SubjectID
                                    select new SubjectsModel
                                    {
                                        SubjectID = s.SubjectID,
                                        SubjectName = s.SubjectName,
                                        Theory = s.Theory,
                                        Practice = s.Practice,
                                        Course = s.Course,
                                        UserID = p.UserID
                                    }).ToList();
                ViewBag.SubjectShare = SubjectShare;
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
        public ActionResult Msubjects()
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
                var ParticipantData = (from p in DB.Participant
                                       join u in DB.UserSystem on p.UserID equals u.UserID
                                       where u.UserID == user.UserID
                                       orderby p.SubjectID
                                       select new ParticipantModel
                                       {
                                           PSubjectID = p.SubjectID,
                                           ParticipantID = p.ParticipantID,
                                           PUserID = p.UserID
                                       }).ToList();
                ViewBag.Participant = ParticipantData;
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
                                       Theory = s.Theory,
                                       Practice = s.Practice,
                                       Course = s.Course,
                                       UserID = s.UserID
                                   }).ToList();
                ViewBag.Subject = SubjectData;
            }
            using (var DB = new dbEntities())
            {
                var SubjectShare = (from s in DB.Subjects
                                    join p in DB.Participant on s.SubjectID equals p.SubjectID
                                    where p.ParticipantID == user.UserID
                                    orderby s.SubjectID
                                    select new SubjectsModel
                                    {
                                        SubjectID = s.SubjectID,
                                        SubjectName = s.SubjectName,
                                        Theory = s.Theory,
                                        Practice = s.Practice,
                                        Course = s.Course,
                                        UserID = p.UserID
                                    }).ToList();
                ViewBag.SubjectShare = SubjectShare;
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
        public JsonResult CreateSubject(SubjectsModel subject)
        {
            var jsonretern = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    if (subject.isUpdateable != 1)
                    {
                        DB.Subjects.Add(new Subjects
                        {
                            SubjectID = subject.SubjectID,
                            UserID = subject.UserID,
                            SubjectName = subject.SubjectName,
                            Theory = subject.Theory,
                            Practice = subject.Practice,
                            Course = subject.Course,
                            Description = subject.Description
                        });
                        DB.SaveChanges();

                    }
                    else
                    {
                        var subjectUpdate = DB.Subjects.Where(x => x.SubjectID == subject.SubjectID ).FirstOrDefault();
                        if (subjectUpdate != null)
                        {
                            DB.Subjects.Where(x => x.SubjectID == subjectUpdate.SubjectID).ForEach(x =>
                           {
                               x.SubjectID = subject.SubjectID;
                               x.UserID = subject.UserID;
                               x.SubjectName = subject.SubjectName;
                               x.Theory = subject.Theory;
                               x.Practice = subject.Practice;
                               x.Course = subject.Course;
                               x.Description = subject.Description;
                           });
                            DB.SaveChanges();
                        }
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
        public JsonResult EditSubject(string id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var EditSub = (from s in DB.Subjects
                                   join u in DB.UserSystem on s.UserID equals u.UserID
                                   where s.SubjectID == id
                                   select new SubjectsModel
                                   {
                                       SubjectID = s.SubjectID,
                                       SubjectName = s.SubjectName,
                                       Theory = s.Theory,
                                       Practice = s.Practice,
                                       Course = s.Course,
                                       Description = s.Description,
                                       UserID = s.UserID,
                                       Fname = u.Fname,
                                       Lname = u.Lname
                                   }).FirstOrDefault();
                    if (EditSub == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย", data = EditSub };
                    }
                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }
        public ActionResult DeleteSubject(string id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var DeleteSub = DB.Subjects.Where(x => x.SubjectID == id).FirstOrDefault();
                    if (DeleteSub == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        DB.Subjects.Remove(DeleteSub);
                        DB.SaveChanges();
                        jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย" };
                    }
                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }
        public JsonResult CreateParticipant(ParticipantModel Participant)
        {
            var jsonretern = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    if (Participant.isUpdateable != 1)
                    {
                        DB.Participant.Add(new Participant
                        {
                            ParticipantID = Participant.ParticipantID,
                            SubjectID = Participant.PSubjectID,
                            UserID = Participant.PUserID
                        });
                        DB.SaveChanges();

                    }
                    else
                    {
                        var ParticipantUpdate = DB.Participant.Where(x => x.ParticipantID == Participant.ParticipantID).FirstOrDefault();
                        if (ParticipantUpdate != null)
                        {
                            DB.Participant.Where(x => x.ParticipantID == ParticipantUpdate.ParticipantID).ForEach(x =>
                            {
                                x.ParticipantID = Participant.ParticipantID;
                                x.SubjectID = Participant.PSubjectID;
                                x.UserID = Participant.PUserID;
                            });
                            DB.SaveChanges();
                        }
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
        public JsonResult EditParticipant(string id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var EditPartici = (from p in DB.Participant
                                       join u in DB.UserSystem on p.UserID equals u.UserID
                                       where p.SubjectID == id
                                       select new ParticipantModel
                                       {
                                           PSubjectID = p.SubjectID,
                                           ParticipantID = p.ParticipantID,
                                           PUserID = p.UserID
                                       }).FirstOrDefault();
                    if (EditPartici == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย", data = EditPartici };
                    }
                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }
        public ActionResult DeleteParticipant(string id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var DeletePartici = DB.Participant.Where(x => x.ParticipantID == id).FirstOrDefault();
                    if (DeletePartici == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        DB.Participant.Remove(DeletePartici);
                        DB.SaveChanges();
                        jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย" };
                    }
                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }
    }
}