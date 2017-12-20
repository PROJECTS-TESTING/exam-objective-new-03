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
    public class TestingController : Controller
    {
        // GET: Testing
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
                ViewBag.Subject = SubjectData;
            }
            return View();
        }
       
        public ActionResult CreateTest(string etid) {
            var user = Session["User"] as UserSystemModel;
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (user.Status == "student")
            {
                return RedirectToAction("Index", "Student");
            }
            //Log.Warning("" + user.UserID + " | " + NetworkClient.GetIPClien());
            using (var DB = new dbEntities())
            {
                var SubjectData = (from s in DB.Subjects
                                   join u in DB.UserSystem on s.UserID equals u.UserID
                                   where u.UserID == user.UserID && s.SubjectID == etid
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

                var GroupData = (from g in DB.TestGroup
                                 where g.UserID == user.UserID && g.SubjectID == etid
                                 select new TestGroupModel
                                 {
                                     GroupID = g.GroupID,
                                     StudyGroup = g.GroupName,
                                     GroupPW = g.GroupPW
                                 }).ToList();
                ViewBag.testGroup = GroupData;
            }
           using(var DB = new dbEntities())
            {
                var DataTest = (from e in DB.ExamTopic
                                join g in DB.TestGroup on e.GroupID equals g.GroupID
                                where e.UserID == user.UserID && e.SubjectID == etid 
                                select new ExamTopicModel
                                {
                                    ExamtopicID = e.ExamtopicID,
                                    ExamtopicName = e.ExamtopicName,
                                    StudyGroup = g.GroupName
                                   
                                }).ToList();

                ViewBag.DataExamtest = DataTest;

                var DataTestNullGroup = (from e in DB.ExamTopic
                                         where e.UserID == user.UserID && e.SubjectID == etid && e.GroupID == 0
                                         select new ExamTopicModel
                                         {
                                             ExamtopicID = e.ExamtopicID,
                                             ExamtopicName = e.ExamtopicName,
                                             StudyGroup = "ยังไม่มี",
                                             GroupID = e.GroupID

                                         }).ToList();
                ViewBag.DataNullGroup = DataTestNullGroup;
            }
            return View();
        }
        
        public JsonResult CreateTesting(ExamTopicModel examtopic)
        {
            var jsonretern = new JsonRespone();
            var user = Session["User"] as UserSystemModel;
            try
            {
                using (var DB = new dbEntities())
                {
                    if (examtopic.isUpdateable != 1)
                    {
                            examtopic.ExamtopicID = DB.ExamTopic.Count() != 0 ? DB.ExamTopic.Max(x => x.ExamtopicID) + 1:1;
                        
                        DB.ExamTopic.Add(new ExamTopic
                        {
                            ExamtopicID = examtopic.ExamtopicID,
                            ExamtopicName = examtopic.ExamtopicName,
                            Explantion = examtopic.Explantion,
                            UserID = user.UserID,
                            SubjectID = examtopic.SubjectID,
                            DatetoBegin = examtopic.DatetoBegin,
                            TimetoBegin = examtopic.TimetoBegin,
                            TimetoEnd = examtopic.TimetoEnd,
                            NewPage = examtopic.NewPage,
                            HowtoPage = examtopic.HowtoPage,
                            Sequences = examtopic.Sequences,
                            GroupID = examtopic.GroupID,
                            ExamtopicPW = examtopic.ExamtopicPW,
                            NumberOfTimes = examtopic.NumberOfTimes,
                            InNetWork = examtopic.InNetWork
                           
                        });
                        DB.SaveChanges();
                    }
                    else
                    {
                        var ExamTopicUpdate = DB.ExamTopic.Where(x => x.ExamtopicID == examtopic.ExamtopicID).FirstOrDefault();
                        if (ExamTopicUpdate != null)
                        {
                            DB.ExamTopic.Where(x => x.ExamtopicID == ExamTopicUpdate.ExamtopicID).ForEach(x =>
                            {
                                x.ExamtopicName = examtopic.ExamtopicName;
                                x.Explantion = examtopic.Explantion;
                                x.DatetoBegin = examtopic.DatetoBegin;
                                x.TimetoBegin = examtopic.TimetoBegin;
                                x.TimetoEnd = examtopic.TimetoEnd;
                                x.NewPage = examtopic.NewPage;
                                x.HowtoPage = examtopic.HowtoPage;
                                x.GroupID = examtopic.GroupID;
                                x.ExamtopicPW = examtopic.ExamtopicPW;
                                x.NumberOfTimes = examtopic.NumberOfTimes;
                                x.InNetWork = examtopic.InNetWork;
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

        public JsonResult EditExamTopic(int id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var Editexamtopic = (from s in DB.ExamTopic
                                         where s.ExamtopicID == id
                                         select new ExamTopicModel
                                         {
                                             ExamtopicName = s.ExamtopicName,
                                             Explantion = s.Explantion,
                                             DatetoBegin = s.DatetoBegin,
                                             TimetoBegin = s.TimetoBegin,
                                             TimetoEnd = s.TimetoEnd,
                                             NewPage = s.NewPage,
                                             HowtoPage = s.HowtoPage,
                                            // Sequences = s.Sequences,
                                             GroupID = s.GroupID,
                                             ExamtopicPW = s.ExamtopicPW,
                                             //NumberOfTimes = s.NumberOfTimes,
                                             InNetWork = s.InNetWork
                                             

                                 }).FirstOrDefault();
                    if (Editexamtopic == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย", data = Editexamtopic };
                    }
                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }

        public JsonResult DeleteExamTopic(int id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var Deleteexamtopic = DB.ExamTopic.Where(x => x.ExamtopicID == id).FirstOrDefault();
                    if (Deleteexamtopic == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        DB.ExamTopic.Remove(Deleteexamtopic);
                        DB.SaveChanges();
                        jsonreturn = new JsonRespone { status = true, message = "ลบเรียบร้อย" };
                    }
                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }

        // แก้ไขแบบทดสอบ
        public ActionResult EditQuiz(int etid, string subid)
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
            using(var DB = new dbEntities())
            {
                if (DB.ExamBody.Where(eb => eb.ExamtopicID == etid).Count() == 0)
                {
                    DB.ExamBody.Add(new ExamBody {
                        ExamBodyID = (DB.ExamBody.Count() != 0) ? DB.ExamBody.Max(e => e.ExamBodyID) + 1 : 1,
                        ExamtopicID = etid,
                       
                    });
                DB.SaveChanges();
                }
                ViewBag.DataExambodyID = (from e in DB.ExamBody where e.ExamtopicID == etid select e.ExamBodyID).FirstOrDefault();
            }
            using (var DB = new dbEntities())
            {
                var SubjectData = (from s in DB.Subjects
                                   join u in DB.UserSystem on s.UserID equals u.UserID
                                   where u.UserID == user.UserID && s.SubjectID == subid
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
                var idG = (from e in DB.ExamTopic where e.ExamtopicID == etid select e.GroupID).FirstOrDefault();

                var GroupData = (from g in DB.TestGroup
                                 where g.GroupID == idG
                                 select new TestGroupModel
                                 {
                                     GroupID = g.GroupID,
                                     StudyGroup = g.GroupName,
                                     GroupPW = g.GroupPW
                                 }).ToList();
                ViewBag.testGroup = GroupData;
            }
            using (var DB = new dbEntities())
            {
                var Examto = (from e in DB.ExamTopic
                              where e.ExamtopicID == etid
                              select new ExamTopicModel
                              {
                                  ExamtopicID = e.ExamtopicID,
                                  ExamtopicName = e.ExamtopicName,
                                  Explantion = e.Explantion
                              }
                              ).ToList();
                ViewBag.examtopic = Examto;
            }
            using (var DB = new dbEntities())
            {
                var LessonData = (from l in DB.Lesson
                                  where user.UserID == l.UserID && l.SubjectID == subid
                                  orderby l.LessonID
                                  select new LessonModel
                                  {
                                      LessonID = l.LessonID,
                                      LesName = l.LesName,
                                      TextLesson = l.TextLesson

                                  }).DistinctBy(r => r.LesName).ToList();
                ViewBag.Lesson = LessonData;
            }
            using(var DB = new dbEntities())
            {
                var countPropo = (from l in DB.Lesson

                                  let countP = (from o in DB.Objective
                                                join p in DB.Proposition on o.ObjID equals p.ObjID
                                                where o.ObjID == p.ObjID && o.LessonID == l.LessonID
                                                select l).Count()

                                  where user.UserID == l.UserID && l.SubjectID == subid
                                  orderby l.LessonID
                                  select new LessonModel
                                  {
                                      LessonID = l.LessonID,
                                      LesName = l.LesName,
                                      TextLesson = l.TextLesson,
                                      CountProposID = countP
                                  }).ToList();
                ViewBag.countProposi = countPropo;
            }
            using(var DB = new dbEntities())
            {
                var DataQuiz = (from l in DB.Lesson
                                join o in DB.Objective on l.LessonID equals o.LessonID
                                join p in DB.Proposition on o.ObjID equals p.ObjID
                                where user.UserID == l.UserID && l.SubjectID == subid
                                select new PropositionModel
                                {
                                    ProposID = p.ProposID,
                                    ProposName = p.ProposName,
                                    LessonID = l.LessonID,
                                    LesName = l.LesName,
                                    ObjID = o.ObjID
                                }
                                ).ToList();
                ViewBag.QuizData = DataQuiz;
            }
            using (var DB = new dbEntities())
            {
                ViewBag.ShowQuiz = (from g in DB.GetExam
                                    join p in DB.Proposition on g.ProposID equals p.ProposID
                                    join o in DB.Objective on p.ObjID equals o.ObjID
                                    join l in DB.Lesson on o.LessonID equals l.LessonID
                                    where g.ExamBodyID == (from e in DB.ExamBody where e.ExamtopicID == etid select e.ExamBodyID).FirstOrDefault()
                                    select new ShowQuizModel
                                    {
                                        ExamBodyID = g.ExamBodyID,
                                        ProposID = g.ProposID,
                                        ProposName = p.ProposName,
                                       LessonID = l.LessonID,
                                       LesName = l.LesName
                                    }).ToList();
            }
                return View();
        }

        public JsonResult AddQuiz(SelectQuizModel selectQ)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using(var DB = new dbEntities())
                {
                    for(var i = 0; i < selectQ.ProposID.Length; i++)
                    {
                        DB.GetExam.Add(new GetExam {ExamBodyID = selectQ.ExamBodyID,ProposID = selectQ.ProposID[i] });DB.SaveChanges();
                    }
                    jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย" };
                }     
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }

        public JsonResult DeleteQuiz(int id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var DeleteQuiz = DB.GetExam.Where(x => x.ProposID == id).FirstOrDefault();
                    if (DeleteQuiz == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        DB.GetExam.Remove(DeleteQuiz);
                        DB.SaveChanges();
                        jsonreturn = new JsonRespone { status = true, message = "ลบเรียบร้อย" };
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