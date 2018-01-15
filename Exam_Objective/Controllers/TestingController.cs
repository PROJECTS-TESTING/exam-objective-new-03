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
                              select new ExamTopicModel {
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
            using(var DB = new dbEntities())
            {
                var dataObj = (from o in DB.Objective
                               join l in DB.Lesson on o.LessonID equals l.LessonID
                               let oPropos = (         
                                          from p in DB.Proposition
                                          where p.ObjID == o.ObjID
                                          select o).Count()
                               where l.UserID == user.UserID && l.SubjectID == subid
                               select new ObjectiveModel {
                                   ObjID = o.ObjID,
                                   ObjName = o.ObjName,
                                   TextObj = o.TextObj,
                                   PLessonID = o.LessonID,
                                   CountProposID = oPropos
                               }).ToList();
                ViewBag.DataObjective = dataObj;
            }
            // รายละเอียดข้อสอบแต่ละข้อในวิชานี้
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
            using(var DB = new dbEntities())
            {
                ViewBag.dataGroup = (from gg in DB.TestGroup
                                     where gg.GroupID == g
                                     select gg.GroupName).FirstOrDefault();
            }
            using(var DB = new dbEntities())
            {
                ViewBag.dataSubject = (from s in DB.Subjects where s.SubjectID == subjid && s.UserID == user.UserID select s.SubjectName).FirstOrDefault();
            }
            using(var DB = new dbEntities())
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
                if (dataextopic[0].Sequences.Equals("1"))
                {
                    dataProp.Shuff();
                }
                ViewBag.dataProposition = dataProp;
            }
            using(var DB = new dbEntities())
            {
                ViewBag.dataChoice = (from ex in DB.GetExam
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
            }
            return View();
        }
        // Check score students
        public ActionResult ScoreStudent(int etid, string subid, int g)
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
                ViewBag.dataSubject = (from s in DB.Subjects where s.SubjectID == subid && s.UserID == user.UserID select s.SubjectName).FirstOrDefault();
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

                foreach(var r in dataUserStudent)
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
                                    if(a.Answer > 0)
                                    score = score + (float)a.Answer;
                                }
                            }
                        }
                    }
                    float scoresper = (score * 100) / dataTestAns.Count;
                   
                    DataScoreStudent.Add(new ScoreStudent
                    { Score = score,
                        Scoreper = scoresper,
                        countQuiz = dataTestAns.Count,
                        Fname = (from u in DB.UserSystem where u.UserID == r.UserID select u.Fname).FirstOrDefault(),
                        Lname = (from u in DB.UserSystem where u.UserID == r.UserID select u.Lname).FirstOrDefault()
                    });
                }
                ViewBag.dataScore = DataScoreStudent;
            }
            return View();
        }
    }
}