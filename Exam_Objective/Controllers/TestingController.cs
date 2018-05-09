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
using NSystems.Collections;
using System.Web.UI.WebControls;
using System.Xml;
using OfficeOpenXml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using NotesFor.HtmlToOpenXml;
using System.Text.RegularExpressions;

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

        public ActionResult CreateTest(string etid)
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
            using (var DB = new dbEntities())
            {
                var DataTest = (from e in DB.ExamTopic
                                join g in DB.TestGroup on e.GroupID equals g.GroupID
                                where e.UserID == user.UserID && e.SubjectID == etid
                                select new ExamTopicModel
                                {
                                    ExamtopicID = e.ExamtopicID,
                                    ExamtopicName = e.ExamtopicName,
                                    StudyGroup = g.GroupName,
                                    GroupID = e.GroupID

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
                        examtopic.ExamtopicID = DB.ExamTopic.Count() != 0 ? DB.ExamTopic.Max(x => x.ExamtopicID) + 1 : 1;

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
                                x.Sequences = examtopic.Sequences;
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
                                             Sequences = s.Sequences,
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

        // --------------- แก้ไขแบบทดสอบ -------------
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
            // สร้างตัวข้อสอบไว้รอ
            using (var DB = new dbEntities())
            {
                if (DB.ExamBody.Where(eb => eb.ExamtopicID == etid).Count() == 0)
                {
                    DB.ExamBody.Add(new ExamBody
                    {
                        ExamBodyID = (DB.ExamBody.Count() != 0) ? DB.ExamBody.Max(e => e.ExamBodyID) + 1 : 1,
                        ExamtopicID = etid,

                    });
                    DB.SaveChanges();
                }
                ViewBag.DataExambodyID = (from e in DB.ExamBody where e.ExamtopicID == etid select e.ExamBodyID).FirstOrDefault();
            }
            // รายละเอียดวิชา
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
            //  รายละเอียดกลุ่มเรียน
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
            // รายละเอียดหัวข้อแบบทดสอบ
            using (var DB = new dbEntities())
            {
                var Examto = (from e in DB.ExamTopic
                              where e.ExamtopicID == etid
                              select new ExamTopicModel
                              {
                                  ExamtopicID = e.ExamtopicID,
                                  ExamtopicName = e.ExamtopicName,
                                  Explantion = e.Explantion
                              }).ToList();
                ViewBag.examtopic = Examto;
            }
            // รายละเอียดบทเรียน
            using (var DB = new dbEntities())
            {
                var LessonData = (from l in DB.Lesson
                                  let countobj = (from o in DB.Objective
                                                  where o.LessonID == l.LessonID
                                                  select l).Count()
                                  where user.UserID == l.UserID && l.SubjectID == subid
                                  orderby l.LessonID
                                  select new LessonModel
                                  {
                                      LessonID = l.LessonID,
                                      LesName = l.LesName,
                                      TextLesson = l.TextLesson,
                                      CountObjective = countobj
                                  }).DistinctBy(r => r.LesName).ToList();
                ViewBag.Lesson = LessonData;
            }
            // วัตถุประสงค์
            using (var DB = new dbEntities())
            {
                var dataObj = (from o in DB.Objective
                               join l in DB.Lesson on o.LessonID equals l.LessonID
                               let oPropos = (
                                          from p in DB.Proposition
                                          where p.ObjID == o.ObjID
                                          select o).Count()
                               where l.UserID == user.UserID && l.SubjectID == subid
                               select new ObjectiveModel
                               {
                                   ObjID = o.ObjID,
                                   ObjName = o.ObjName,
                                   TextObj = o.TextObj,
                                   PLessonID = o.LessonID,
                                   CountProposID = oPropos
                               }).ToList();
                ViewBag.DataObjective = dataObj;
            }
            // รายละเอียดข้อสอบแต่ละข้อในวิชานี้
            using (var DB = new dbEntities())
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
                                    ObjID = p.ObjID,
                                    Difficulty = p.Difficulty
                                }
                                ).ToList();
                ViewBag.QuizData = DataQuiz;
            }
            // แสดงข้อสอบที่มีการเลือกแล้ว
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
                using (var DB = new dbEntities())
                {
                    for (var i = 0; i < selectQ.ProposID.Length; i++)
                    {
                        DB.GetExam.Add(new GetExam { ExamBodyID = selectQ.ExamBodyID, ProposID = selectQ.ProposID[i] }); DB.SaveChanges();
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

        public JsonResult ViewPropos(int idp)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    int CountChoice = DB.Choice.Where(x => x.ProposID == idp).Count();
                    string[] TChoice = new string[6];
                    for (int i = 1; i <= CountChoice; i++)
                    {
                        TChoice[i] = (from c in DB.Choice
                                      where c.ProposID == idp && c.ChoiceID == i
                                      select c.TextChoice
                                     ).FirstOrDefault();
                    }
                    var TextChoice1 = TChoice[1];
                    var TextChoice2 = TChoice[2];
                    var TextChoice3 = TChoice[3];
                    var TextChoice4 = TChoice[4];
                    var TextChoice5 = TChoice[5];
                    var EditPropos = (from p in DB.Proposition
                                      join c in DB.Proposition on p.ProposID equals c.ProposID
                                      where p.ProposID == idp
                                      select new PropositionModel
                                      {
                                          ProposID = p.ProposID,
                                          ProposName = p.ProposName,
                                          TextPropos = p.TextPropos,
                                          ScoreMain = p.ScoreMain,
                                          CheckChoice = p.CheckChoice,
                                          Choice1 = TextChoice1,
                                          Choice2 = TextChoice2,
                                          Choice3 = TextChoice3,
                                          Choice4 = TextChoice4,
                                          Choice5 = TextChoice5

                                      }).FirstOrDefault();

                    if (EditPropos == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย", data = EditPropos };
                    }
                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }

        public ActionResult ViewTesting(string subjid, int exbody, int extopic, int g)
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
                ViewBag.dataGroup = (from gg in DB.TestGroup
                                     where gg.GroupID == g
                                     select gg.GroupName).FirstOrDefault();
            }
            using (var DB = new dbEntities())
            {
                ViewBag.dataSubject = (from s in DB.Subjects where s.SubjectID == subjid && s.UserID == user.UserID select s.SubjectName).FirstOrDefault();
            }
            using (var DB = new dbEntities())
            {
                var dataextopic = (from e in DB.ExamTopic
                                   where e.ExamtopicID == extopic
                                   select new ExamTopicModel
                                   {
                                       ExamtopicID = e.ExamtopicID,
                                       ExamtopicName = e.ExamtopicName,
                                       DatetoBegin = e.DatetoBegin,
                                       TimetoBegin = e.TimetoBegin,
                                       TimetoEnd = e.TimetoEnd,
                                       Sequences = e.Sequences

                                   }).ToList();
                ViewBag.DataExamtopic = dataextopic;

                var dataProp = (from ex in DB.GetExam
                                join p in DB.Proposition on ex.ProposID equals p.ProposID
                                where ex.ExamBodyID == exbody
                                select new PropositionModel
                                {
                                    ProposID = ex.ProposID,
                                    ProposName = p.ProposName,
                                    TextPropos = p.TextPropos,
                                    Continuity = p.Continuity
                                }).ToList();
                if (dataextopic[0].Sequences.Equals("1") || dataextopic[0].Sequences.Equals("3"))
                {
                    dataProp.Shuff();
                }
                ViewBag.dataProposition = dataProp;

                var dataChoices = (from ex in DB.GetExam
                                   join c in DB.Choice on ex.ProposID equals c.ProposID
                                   let countans = (from ca in DB.Choice where ca.ProposID == ex.ProposID && ca.Answer > 0 select ex).Count()
                                   where ex.ExamBodyID == exbody
                                   orderby c.ChoiceID
                                   select new ChoiceModel
                                   {
                                       ProposID = ex.ProposID,
                                       ChoiceID = c.ChoiceID,
                                       TextChoice = c.TextChoice,
                                       Answer = c.Answer,
                                       countAnswer = countans
                                   }).ToList();

                if (dataextopic[0].Sequences.Equals("2") || dataextopic[0].Sequences.Equals("3"))
                {
                    List<int> checkChoice = new List<int>();
                    foreach (var datac in dataChoices)
                    {
                        if (datac.ChoiceID == 4 && datac.TextChoice.Length > 13)
                        {
                            if (datac.TextChoice.Substring(3, 7).Equals("ถูกทั้ง"))
                            {
                                checkChoice.Add(datac.ProposID);
                            }
                            else if (datac.TextChoice.Substring(3, 9).Equals("ถูกทุกข้อ"))
                            {
                                checkChoice.Add(datac.ProposID);
                            }
                            else if (datac.TextChoice.Length > 13 && datac.TextChoice.Substring(3, 11).Equals("ไม่มีข้อถูก"))
                            {
                                checkChoice.Add(datac.ProposID);
                            }
                            else if (datac.TextChoice.Length > 15 && datac.TextChoice.Substring(3, 13).Equals("ไม่มีข้อใดถูก"))
                            {
                                checkChoice.Add(datac.ProposID);
                            }
                        }
                    }
                    ViewBag.dataCheck = checkChoice;
                }

                ViewBag.dataChoice = dataChoices;
            }
            return View();
        }
        // Check score students
        public ActionResult ScoreStudent(int etid, string subid, int g)
        {
            ViewBag.etid = etid;
            ViewBag.subid = subid;
            ViewBag.groupid = g;
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
                ViewBag.dataSubject = (from s in DB.Subjects where s.SubjectID == subid && s.UserID == user.UserID select s.SubjectName).FirstOrDefault();
            }
            using (var DB = new dbEntities())
            {
                ViewBag.dataGroup = (from gr in DB.TestGroup where gr.GroupID == g select gr.GroupName).FirstOrDefault();
            }
            using (var DB = new dbEntities())
            {
                ViewBag.DataExamtopic = (from e1 in DB.ExamTopic
                                         where e1.ExamtopicID == etid
                                         select new ExamTopicModel
                                         {
                                             ExamtopicID = e1.ExamtopicID,
                                             ExamtopicName = e1.ExamtopicName
                                         }).ToList();
            }
            // หารายละเอียดผลคะแนนการสอบของนักศึกษา
            using (var DB = new dbEntities())
            {
                List<ScoreStudent> DataScoreStudent = new List<ScoreStudent>();
                var dataUserStudent = (from t in DB.Testing
                                       where t.ExamBodyID == (from eb in DB.ExamBody where eb.ExamtopicID == etid select eb.ExamBodyID).FirstOrDefault()
                                       select new { t.ExamBodyID, t.UserID }).DistinctBy(e => e.UserID).ToList();

                foreach (var r in dataUserStudent)
                {
                    int numberoftime = (from t in DB.Testing
                                        where t.ExamBodyID == r.ExamBodyID && t.UserID == r.UserID
                                        select t.NumberOfTimes).Max();
                    var dataTesting = (from t in DB.Testing
                                       join c in DB.Choice on t.ProposID equals c.ProposID
                                       where t.ExamBodyID == r.ExamBodyID && t.UserID == r.UserID && t.NumberOfTimes == numberoftime
                                       select new { c.Answer, t.ProposID, c.ChoiceID }).ToList();
                    var dataTestAns = DB.Testing.Where(t => t.ExamBodyID == r.ExamBodyID && t.UserID == r.UserID && t.NumberOfTimes == numberoftime).ToList();
                    float score = 0f;
                    foreach (var c in dataTestAns)
                    {
                        if (c.AnswerStudent.Length > 1)
                        {
                            string[] strchoice = c.AnswerStudent.Split(',');
                            for (var i = 0; i < strchoice.Length; i++)
                            {
                                foreach (var a in dataTesting)
                                {
                                    if (c.ProposID == a.ProposID && a.ChoiceID == Int16.Parse(strchoice[i]))
                                    {
                                        score = a.Answer > 0 ? score + (float)a.Answer : score - 0.5f;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var a in dataTesting)
                            {
                                if (c.ProposID == a.ProposID && c.AnswerStudent.Equals(a.ChoiceID.ToString()))
                                {
                                    if (a.Answer > 0)
                                        score = score + (float)a.Answer;
                                }
                            }
                        }
                    }
                    float scoresper = (score * 100) / dataTestAns.Count;

                    DataScoreStudent.Add(new ScoreStudent
                    {
                        Score = score,
                        Scoreper = scoresper,
                        countQuiz = dataTestAns.Count,
                        Fname = (from u in DB.UserSystem where u.UserID == r.UserID select u.Fname).FirstOrDefault(),
                        Lname = (from u in DB.UserSystem where u.UserID == r.UserID select u.Lname).FirstOrDefault(),
                        StudentID = (from u in DB.UserSystem where u.UserID == r.UserID select u.StudentID).FirstOrDefault()
                    });
                }
                ViewBag.dataScore = DataScoreStudent;
            }
            return View();
        }
        // สร้าง Excel File รายงานผลคะแนนนักศึกษา
        public void ExportScore(int etid, string subid, int g)
        {
            var user = Session["User"] as UserSystemModel;
            var subjectName = "";
            var groupName = "";
            var ExamtopicName = "";
            List<ScoreStudent> DataScoreStudent = new List<ScoreStudent>();
            using (var DB = new dbEntities())
            {
                subjectName = (from s in DB.Subjects where s.SubjectID == subid && s.UserID == user.UserID select s.SubjectName).FirstOrDefault();
                groupName = (from gr in DB.TestGroup where gr.GroupID == g select gr.GroupName).FirstOrDefault();
                ExamtopicName = (from e1 in DB.ExamTopic where e1.ExamtopicID == etid select e1.ExamtopicName).FirstOrDefault();

                var dataUserStudent = (from t in DB.Testing
                                       where t.ExamBodyID == (from eb in DB.ExamBody where eb.ExamtopicID == etid select eb.ExamBodyID).FirstOrDefault()
                                       select new { t.ExamBodyID, t.UserID }).DistinctBy(e => e.UserID).ToList();

                foreach (var r in dataUserStudent)
                {
                    var dataTesting = (from t in DB.Testing
                                       join c in DB.Choice on t.ProposID equals c.ProposID
                                       where t.ExamBodyID == r.ExamBodyID && t.UserID == r.UserID
                                       select new { c.Answer, t.ProposID, c.ChoiceID }).ToList();
                    var dataTestAns = DB.Testing.Where(t => t.ExamBodyID == r.ExamBodyID && t.UserID == r.UserID).ToList();
                    float score = 0f;
                    foreach (var c in dataTestAns)
                    {
                        if (c.AnswerStudent.Length > 1)
                        {
                            string[] strchoice = c.AnswerStudent.Split(',');
                            for (var i = 0; i < strchoice.Length; i++)
                            {
                                foreach (var a in dataTesting)
                                {
                                    if (c.ProposID == a.ProposID && a.ChoiceID == Int16.Parse(strchoice[i]))
                                    {
                                        score = a.Answer > 0 ? score + (float)a.Answer : score - 0.5f;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var a in dataTesting)
                            {
                                if (c.ProposID == a.ProposID && c.AnswerStudent.Equals(a.ChoiceID.ToString()))
                                {
                                    if (a.Answer > 0)
                                        score = score + (float)a.Answer;
                                }
                            }
                        }
                    }
                    float scoresper = (score * 100) / dataTestAns.Count;

                    DataScoreStudent.Add(new ScoreStudent
                    {
                        Score = score,
                        Scoreper = scoresper,
                        countQuiz = dataTestAns.Count,
                        Fname = (from u in DB.UserSystem where u.UserID == r.UserID select u.Fname).FirstOrDefault(),
                        Lname = (from u in DB.UserSystem where u.UserID == r.UserID select u.Lname).FirstOrDefault(),
                        StudentID = (from u in DB.UserSystem where u.UserID == r.UserID select u.StudentID).FirstOrDefault()
                    });
                }
            }

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            // Header ExCEL FILE
            ws.Cells["A1"].Value = "วิชา";
            ws.Cells["B1"].Value = subjectName;
            ws.Cells["A2"].Value = "ชื่อแบบทดสอบ";
            ws.Cells["B2"].Value = ExamtopicName;
            ws.Cells["A3"].Value = "จำนวน";
            ws.Cells["B3"].Value = DataScoreStudent[0].countQuiz + " ข้อ";
            ws.Cells["A4"].Value = "กลุ่มเรียน";
            ws.Cells["B4"].Value = groupName;
            ws.Cells["A5"].Value = "อาจารย์ผู้สอน";
            ws.Cells["B5"].Value = user.Fname + " " + user.Lname;
            ws.Cells["A6"].Value = "วันที่";
            ws.Cells["B6"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);
            ws.Cells["A8"].Value = "รหัสนักศึกษา";
            ws.Cells["B8"].Value = "ชื่อ-นามสกุล";
            ws.Cells["C8"].Value = "จำนวนข้อถูก";
            ws.Cells["D8"].Value = "คะแนนเปอร์เซ็น";

            int rowNuber = 9;
            foreach (var dataScore in DataScoreStudent)
            {
                ws.Cells[string.Format("A{0}", rowNuber)].Value = dataScore.StudentID;
                ws.Cells[string.Format("B{0}", rowNuber)].Value = dataScore.Fname + " " + dataScore.Lname;
                ws.Cells[string.Format("C{0}", rowNuber)].Value = string.Format("{0:0.00}", dataScore.Score);
                ws.Cells[string.Format("D{0}", rowNuber)].Value = string.Format("{0:0.00}", dataScore.Scoreper);
                rowNuber++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + ExamtopicName + "-" + subjectName + "-" + groupName + ".xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.Flush();
            Response.End();

        }
        // Export File .CSV .xls .xlsx .dat .txt 
        public void ExportForAnalyze(int etid, string subid, string fileExtension)
        {
            var user = Session["User"] as UserSystemModel;
            var subjectName = "";
            var ExamtopicName = "";

            StringBuilder csvDefault = new StringBuilder();
            StringBuilder txtDefault = new StringBuilder();
            ExcelPackage xlPackage = new ExcelPackage();
            ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Inventory");

            using (var DB = new dbEntities())
            {
                subjectName = (from s in DB.Subjects where s.SubjectID == subid && s.UserID == user.UserID select s.SubjectName).FirstOrDefault();
                ExamtopicName = (from e1 in DB.ExamTopic where e1.ExamtopicID == etid select e1.ExamtopicName).FirstOrDefault();

                var dataUserStudent = (from t in DB.Testing
                                       where t.ExamBodyID == (from eb in DB.ExamBody where eb.ExamtopicID == etid select eb.ExamBodyID).FirstOrDefault()
                                       select new { t.ExamBodyID, t.UserID }).DistinctBy(e => e.UserID).ToList();

                var dataAnswers = (from ge in DB.GetExam
                                   where ge.ExamBodyID == (from eb in DB.ExamBody where eb.ExamtopicID == etid select eb.ExamBodyID).FirstOrDefault()
                                   orderby ge.ProposID
                                   select new { ge.ProposID }
                                   ).ToList();

                int xlRows = 1, xlCol = 1;
                foreach (var c in dataAnswers)
                {
                    csvDefault.Append("" + c.ProposID + ",");
                    txtDefault.Append("" + c.ProposID + ",");
                    worksheet.Cells[1, xlCol].Value = c.ProposID;
                    xlCol++;
                }

                csvDefault.Append("\r\n");
                txtDefault.Append("\r\n");
                xlCol = 1;
                foreach (var c in dataAnswers)
                {
                    csvDefault.Append((from ch in DB.Choice where ch.ProposID == c.ProposID && ch.Answer == 1 select ch.ChoiceID).FirstOrDefault() + ",");
                    txtDefault.Append((from ch in DB.Choice where ch.ProposID == c.ProposID && ch.Answer == 1 select ch.ChoiceID).FirstOrDefault());
                    worksheet.Cells[2, xlCol].Value = (from ch in DB.Choice where ch.ProposID == c.ProposID && ch.Answer == 1 select ch.ChoiceID).FirstOrDefault();
                    xlCol++;
                }

                xlCol = 1; xlRows = 3;
                foreach (var r in dataUserStudent)
                {
                    csvDefault.Append("\r\n");
                    txtDefault.Append("\r\n");
                    var dataTesting = (from t in DB.Testing
                                       where t.ExamBodyID == r.ExamBodyID && t.UserID == r.UserID
                                       orderby t.ProposID
                                       select new { t.ProposID, t.AnswerStudent }).ToList();


                    foreach (var c in dataTesting)
                    {
                        if (c.AnswerStudent.Length > 1)
                        {
                            csvDefault.Append("0,");
                            txtDefault.Append("0");
                            worksheet.Cells[xlRows, xlCol].Value = 0;
                        }
                        else
                        {
                            csvDefault.Append(c.AnswerStudent + ",");
                            txtDefault.Append(c.AnswerStudent);
                            worksheet.Cells[xlRows, xlCol].Value = Int32.Parse(c.AnswerStudent);
                        }
                        xlCol++;
                    }
                    xlRows++;
                    xlCol = 1;
                }

            }

            if (fileExtension.Equals("txt") || fileExtension.Equals("dat"))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(txtDefault.ToString());
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.AddHeader("content-disposition", "attachment; filename=" + ExamtopicName.ToString() + "-" + subjectName + "." + fileExtension.ToString());
                Response.BinaryWrite(bytes);
            }
            else if (fileExtension.Equals("csv"))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(csvDefault.ToString());
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.AddHeader("content-disposition", "attachment; filename=" + ExamtopicName.ToString() + "-" + subjectName + ".csv");
                Response.BinaryWrite(bytes);
            }
            else if (fileExtension.Equals("xlsx"))
            {
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + ExamtopicName.ToString() + "-" + subjectName + ".xlsx");
                Response.BinaryWrite(xlPackage.GetAsByteArray());
            }
            Response.Flush();
            Response.End();
        }

        // --- Hard-Copy ---
        public ActionResult HardCopys(int etid, string subid, int g)
        {
            var user = Session["User"] as UserSystemModel;
            List<HardCopyModel> h = new List<HardCopyModel>();

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
                ViewBag.dataSubject = (from s in DB.Subjects where s.SubjectID == subid && s.UserID == user.UserID select s.SubjectName).FirstOrDefault();
            }
            using (var DB = new dbEntities())
            {
                ViewBag.dataGroup = (from gr in DB.TestGroup where gr.GroupID == g select gr.GroupName).FirstOrDefault();
                var exbodyid = (from b in DB.ExamBody where b.ExamtopicID == etid select b.ExamBodyID).FirstOrDefault();
                h.Add(new HardCopyModel { ExamtopicID = etid, SubjectID = subid, ExamBodyID = exbodyid });
                ViewBag.dataEx = h.ToList();
            }
            return View();
        }

        // Hard-Copy Get file.    

        public void ExportDoc(int etid, string subid, int exbo)
        {
            var user = Session["User"] as UserSystemModel;
            var jsonreturn = new JsonRespone();

            try
            {
                string filename = "Word_Test_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".docx";
                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                StringBuilder html = new StringBuilder();

                html.Append("<body>");

                using (var DB = new dbEntities())
                {
                    var dataProp = (from ex in DB.GetExam
                                    join p in DB.Proposition on ex.ProposID equals p.ProposID
                                    where ex.ExamBodyID == exbo
                                    select new PropositionModel
                                    {
                                        ProposID = ex.ProposID,
                                        ProposName = p.ProposName,
                                        TextPropos = p.TextPropos,
                                        Continuity = p.Continuity
                                    }).ToList();

                    var dataChoices = (from ex in DB.GetExam
                                       join c in DB.Choice on ex.ProposID equals c.ProposID
                                       let countans = (from ca in DB.Choice where ca.ProposID == ex.ProposID && ca.Answer > 0 select ex).Count()
                                       where ex.ExamBodyID == exbo
                                       orderby c.ChoiceID
                                       select new ChoiceModel
                                       {
                                           ProposID = ex.ProposID,
                                           ChoiceID = c.ChoiceID,
                                           TextChoice = c.TextChoice,
                                           Answer = c.Answer,
                                           countAnswer = countans
                                       }).ToList();
                    var datasubject = (from s in DB.Subjects
                                       where s.SubjectID == subid && s.UserID == user.UserID
                                       select new SubjectsModel
                                       {
                                           SubjectID = s.SubjectID,
                                           SubjectName = s.SubjectName
                                       }).ToList();
                    var dataExto = (from e in DB.ExamTopic
                                    where e.ExamtopicID == etid
                                    select new ExamTopicModel
                                    {
                                        ExamtopicName = e.ExamtopicName,
                                        DatetoBegin = e.DatetoBegin,
                                        TimetoBegin = e.TimetoBegin,
                                        TimetoEnd = e.TimetoEnd
                                    }).ToList();

                    string[] mouth_TH = { "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" };
                    string timebegin = dataExto[0].TimetoBegin.Hours + "." + dataExto[0].TimetoBegin.Minutes;
                    string timeend = dataExto[0].TimetoEnd.Hours + "." + dataExto[0].TimetoEnd.Minutes;
                    // Header file test
                    html.Append("<p>ข้อสอบวิชา" + datasubject[0].SubjectName + "&nbsp;(" + datasubject[0].SubjectID + ")&nbsp;สาขาวิชาวิศวกรรมคอมพิวเตอร์</p>");
                    html.Append("<p>คณะวิศวกรรมศาสตร์และสถาปัตยกรรมศาสตร์&nbsp;มหาวิทยาลัยเทคโนโลยีราชมงคลอีสาน</p>");
                    html.Append("<p>" + dataExto[0].ExamtopicName + "&nbsp;วันที่&nbsp;" + dataExto[0].DatetoBegin.Day + "&nbsp;" + mouth_TH[dataExto[0].DatetoBegin.Month - 1] + "&nbsp;" + dataExto[0].DatetoBegin.Year + "&nbsp;&nbsp;เวลา&nbsp;" + timebegin + "-" + timeend + "&nbsp;น</p>");
                    html.Append("<p>____________________________________________________________________________________</p>");
                    var i = 0;
                    var j = 0;
                    foreach (var row in dataProp)
                    {

                        if (row.Continuity == null)
                        {
                            i++;
                            if (Regex.IsMatch(row.TextPropos, "<img.*?>"))
                            {
                                html.Append("<p>" + i + ". " + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + "</p>" + "<p>" + Regex.Match(row.TextPropos, "<img.*?/>") + "</p>");
                            }
                            else
                            {
                                html.Append("<p>" + i + ". " + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + "</p>");
                            }

                            foreach (var rowc in dataChoices)
                            {
                                if (row.ProposID == rowc.ProposID)
                                {
                                    j++;
                                    html.Append("<p>&nbsp;&nbsp;" + j + ". " + rowc.TextChoice.Substring(3, rowc.TextChoice.Length - 3));

                                }
                            }
                            j = 0;
                        }
                        else if (row.Continuity == 0)
                        {

                            var countProp = DB.Proposition.Where(x => x.Continuity == row.ProposID).Count();
                            countProp = i + countProp;
                            if (Regex.IsMatch(row.TextPropos, "<img.*?>"))
                            {
                                html.Append("<p>" + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + (i + 1) + "-" + countProp + "</p>" + "<p>" + Regex.Match(row.TextPropos, "<img.*?/>") + "</p>");
                            }
                            else
                            {
                                html.Append("<p>" + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + (i + 1) + "-" + countProp + "</p>");
                            }

                            foreach (var prop in dataProp)
                            {

                                if (prop.Continuity == row.ProposID)
                                {
                                    i++;
                                    if (Regex.IsMatch(row.TextPropos, "<img.*?>"))
                                    {
                                        html.Append("<p>" + i + ". " + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + "</p>" + "<p>" + Regex.Match(row.TextPropos, "<img.*?/>") + "</p>");
                                    }
                                    else
                                    {
                                        html.Append("<p>" + i + ". " + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + "</p>");
                                    }

                                    foreach (var rowc in dataChoices)
                                    {
                                        if (prop.ProposID == rowc.ProposID)
                                        {
                                            j++;
                                            html.Append("<p>&nbsp;&nbsp;" + j + ". " + rowc.TextChoice.Substring(3, rowc.TextChoice.Length - 3));

                                        }
                                    }
                                    j = 0;
                                }
                            }
                        }
                    }

                }

                html.Append("</body>");
                // Create file Document .docx                            
                using (MemoryStream generatedDocument = new MemoryStream())
                {
                    using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = package.MainDocumentPart;
                        if (mainPart == null)
                        {
                            mainPart = package.AddMainDocumentPart();
                            new Document(new Body()).Save(mainPart);
                        }

                        HtmlConverter converter = new HtmlConverter(mainPart);
                        converter.BaseImageUrl = new Uri(Request.Url.Scheme + "://" + Request.Url.Authority);

                        Body body = mainPart.Document.Body;



                        var paragraphs = converter.Parse(html.ToString());

                        for (int i = 0; i < paragraphs.Count; i++)
                        {
                            body.Append(paragraphs[i]);
                        }

                        mainPart.Document.Save();

                    }

                    byte[] bytesInStream = generatedDocument.ToArray(); // simpler way of converting to array
                    generatedDocument.Close();

                    Response.Clear();
                    Response.ContentType = contentType;
                    Response.AddHeader("content-disposition", "attachment;filename=" + filename);

                    //this will generate problems
                    Response.BinaryWrite(bytesInStream);
                    try
                    {
                        Response.Flush();
                        Response.End();
                    }
                    catch (Exception ex)
                    {
                        //Response.End(); generates an exception. if you don't use it, you get some errors when Word opens the file...
                    }

                }


            }
            catch (Exception ex)
            {

            }
        }

        // AnswerHardCopy
        public void ExportDocans(int etid, string subid, int exbo)
        {
            var user = Session["User"] as UserSystemModel;
            var jsonreturn = new JsonRespone();

            try
            {
                string filename = "Word_Answer_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".docx";
                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                StringBuilder html = new StringBuilder();

                html.Append("<body>");

                using (var DB = new dbEntities())
                {
                    var dataProp = (from ex in DB.GetExam
                                    join p in DB.Proposition on ex.ProposID equals p.ProposID
                                    where ex.ExamBodyID == exbo
                                    select new PropositionModel
                                    {
                                        ProposID = ex.ProposID,
                                        ProposName = p.ProposName,
                                        TextPropos = p.TextPropos,
                                        Continuity = p.Continuity
                                    }).ToList();

                    var dataChoices = (from ex in DB.GetExam
                                       join c in DB.Choice on ex.ProposID equals c.ProposID
                                       let countans = (from ca in DB.Choice where ca.ProposID == ex.ProposID && ca.Answer > 0 select ex).Count()
                                       where ex.ExamBodyID == exbo
                                       orderby c.ChoiceID
                                       select new ChoiceModel
                                       {
                                           ProposID = ex.ProposID,
                                           ChoiceID = c.ChoiceID,
                                           TextChoice = c.TextChoice,
                                           Answer = c.Answer,
                                           countAnswer = countans
                                       }).ToList();
                    var datasubject = (from s in DB.Subjects
                                       where s.SubjectID == subid && s.UserID == user.UserID
                                       select new SubjectsModel
                                       {
                                           SubjectID = s.SubjectID,
                                           SubjectName = s.SubjectName
                                       }).ToList();
                    var dataExto = (from e in DB.ExamTopic
                                    where e.ExamtopicID == etid
                                    select new ExamTopicModel
                                    {
                                        ExamtopicName = e.ExamtopicName,
                                        DatetoBegin = e.DatetoBegin,
                                        TimetoBegin = e.TimetoBegin,
                                        TimetoEnd = e.TimetoEnd
                                    }).ToList();

                    string[] mouth_TH = { "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" };
                    string timebegin = dataExto[0].TimetoBegin.Hours + "." + dataExto[0].TimetoBegin.Minutes;
                    string timeend = dataExto[0].TimetoEnd.Hours + "." + dataExto[0].TimetoEnd.Minutes;
                    // Header file test
                    html.Append("<p>เฉลยข้อสอบวิชา" + datasubject[0].SubjectName + "&nbsp;(" + datasubject[0].SubjectID + ")&nbsp;สาขาวิชาวิศวกรรมคอมพิวเตอร์</p>");
                    html.Append("<p>คณะวิศวกรรมศาสตร์และสถาปัตยกรรมศาสตร์&nbsp;มหาวิทยาลัยเทคโนโลยีราชมงคลอีสาน</p>");
                    html.Append("<p>" + dataExto[0].ExamtopicName + "&nbsp;วันที่&nbsp;" + dataExto[0].DatetoBegin.Day + "&nbsp;" + mouth_TH[dataExto[0].DatetoBegin.Month - 1] + "&nbsp;" + dataExto[0].DatetoBegin.Year + "&nbsp;&nbsp;เวลา&nbsp;" + timebegin + "-" + timeend + "&nbsp;น</p>");
                    html.Append("<p>____________________________________________________________________________________</p>");

                    var i = 0;
                    var j = 0;
                    foreach (var row in dataProp)
                    {

                        if (row.Continuity == null)
                        {
                            i++;
                            if (Regex.IsMatch(row.TextPropos, "<img.*?>"))
                            {
                                html.Append("<p>" + i + ". " + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + "</p>" + "<p>" + Regex.Match(row.TextPropos, "<img.*?/>") + "</p>");
                            }
                            else
                            {
                                html.Append("<p>" + i + ". " + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + "</p>");
                            }

                            foreach (var rowc in dataChoices)
                            {
                                if (row.ProposID == rowc.ProposID)
                                {
                                    // j++;
                                    if (rowc.Answer > 0)
                                    {
                                        html.Append("<p>&nbsp;&nbsp;ตอบ" + rowc.ChoiceID + ". " + rowc.TextChoice.Substring(3, rowc.TextChoice.Length - 3));
                                    }
                                }
                            }
                            j = 0;
                        }
                        else if (row.Continuity == 0)
                        {

                            var countProp = DB.Proposition.Where(x => x.Continuity == row.ProposID).Count();
                            countProp = i + countProp;
                            if (Regex.IsMatch(row.TextPropos, "<img.*?>"))
                            {
                                html.Append("<p>" + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + (i + 1) + "-" + countProp + "</p>" + "<p>" + Regex.Match(row.TextPropos, "<img.*?/>") + "</p>");
                            }
                            else
                            {
                                html.Append("<p>" + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + (i + 1) + "-" + countProp + "</p>");
                            }

                            foreach (var prop in dataProp)
                            {

                                if (prop.Continuity == row.ProposID)
                                {
                                    i++;
                                    if (Regex.IsMatch(row.TextPropos, "<img.*?>"))
                                    {
                                        html.Append("<p>" + i + ". " + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + "</p>" + "<p>" + Regex.Match(row.TextPropos, "<img.*?/>") + "</p>");
                                    }
                                    else
                                    {
                                        html.Append("<p>" + i + ". " + Regex.Replace(row.TextPropos, "<.*?>", String.Empty) + "</p>");
                                    }

                                    foreach (var rowc in dataChoices)
                                    {
                                        if (prop.ProposID == rowc.ProposID)
                                        {
                                            // j++;
                                            if (rowc.Answer > 0)
                                            {
                                                html.Append("<p>&nbsp;&nbsp;ตอบ " + rowc.ChoiceID + ". " + rowc.TextChoice.Substring(3, rowc.TextChoice.Length - 3));
                                            }
                                        }
                                    }
                                    j = 0;
                                }
                            }
                        }
                    }

                }

                html.Append("</body>");
                // Create file Document .docx
                using (MemoryStream generatedDocument = new MemoryStream())
                {
                    using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = package.MainDocumentPart;
                        if (mainPart == null)
                        {
                            mainPart = package.AddMainDocumentPart();
                            new Document(new Body()).Save(mainPart);
                        }

                        HtmlConverter converter = new HtmlConverter(mainPart);
                        converter.BaseImageUrl = new Uri(Request.Url.Scheme + "://" + Request.Url.Authority);

                        Body body = mainPart.Document.Body;



                        var paragraphs = converter.Parse(html.ToString());

                        for (int i = 0; i < paragraphs.Count; i++)
                        {
                            body.Append(paragraphs[i]);
                        }

                        mainPart.Document.Save();

                    }

                    byte[] bytesInStream = generatedDocument.ToArray(); // simpler way of converting to array
                    generatedDocument.Close();

                    Response.Clear();
                    Response.ContentType = contentType;
                    Response.AddHeader("content-disposition", "attachment;filename=" + filename);

                    //this will generate problems
                    Response.BinaryWrite(bytesInStream);
                    try
                    {
                        Response.Flush();
                        Response.End();
                    }
                    catch (Exception ex)
                    {
                        //Response.End(); generates an exception. if you don't use it, you get some errors when Word opens the file...
                    }

                }


            }
            catch (Exception ex)
            {

            }
        }
    }
}