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
    public class ExamBankController : Controller
    {
        // GET: ExamBank         

        public ActionResult CreateLesson_Obj(string SubjecID)
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
        // CreateLesson
        public JsonResult CreateLesson(LessonModel lesson)
        {
            var jsonretern = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    if (lesson.LessonID == 0) //ในกรณีที่เป็น Auto increment ค่าเริ่มต้นเป็น 0  ถ้า PK ไม่เป็น auto incresment ก็ให้ใช้ != 1 และกำหนด public int isUpdateable { get; set; } ในโมเดล เพื่อเช็ค
                    {
                        DB.Lesson.Add(new Lesson
                        {
                            LessonID = lesson.LessonID,
                            LesName = lesson.LesName,
                            SubjectID = lesson.SubjectID,
                            TextLesson = lesson.TextLesson,
                            UserID = lesson.UserID
                        });
                        DB.SaveChanges();

                    }
                    else
                    {
                        var lessonUpdate = DB.Lesson.Where(x => x.LessonID == lesson.LessonID).FirstOrDefault();
                        if (lessonUpdate != null)
                        {
                            DB.Lesson.Where(x => x.LessonID == lessonUpdate.LessonID).ForEach(x =>
                            {
                                x.LessonID = lesson.LessonID;
                                x.LesName = lesson.LesName;
                                x.SubjectID = lesson.SubjectID;
                                x.TextLesson = lesson.TextLesson;
                                x.UserID = lesson.UserID;
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
        // CreateObj
        public JsonResult CreateObj(ObjectiveModel Objective)
        {
            var jsonretern = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    if (Objective.ObjID == 0) //ในกรณีที่เป็น Auto increment ค่าเริ่มต้นเป็น 0  ถ้า PK ไม่เป็น auto incresment ก็ให้ใช้ != 1 และกำหนด public int isUpdateable { get; set; } ในโมเดล เพื่อเช็ค
                    {
                        DB.Objective.Add(new Objective
                        {
                            ObjID = Objective.ObjID,
                            ObjName = Objective.ObjName,
                            TextObj = Objective.TextObj,
                            LessonID = Objective.PLessonID
                        });
                        DB.SaveChanges();

                    }
                    else
                    {
                        var ObjUpdate = DB.Objective.Where(x => x.ObjID == Objective.ObjID).FirstOrDefault();
                        if (ObjUpdate != null)
                        {
                            DB.Objective.Where(x => x.ObjID == ObjUpdate.ObjID).ForEach(x =>
                            {
                                x.ObjID = Objective.ObjID;
                                x.ObjName = Objective.ObjName;
                                x.TextObj = Objective.TextObj;
                                x.LessonID = Objective.PLessonID;

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
        // GEGIN Propos
        // Index of Proposition
        public ActionResult Propos()
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
                                        UserID = p.UserID
                                    }).ToList();
                ViewBag.SubjectShare = SubjectShare;
            }

            return View();
        }
        public ActionResult Proposview(string SubjecID)
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
                var ObjectiveDatanotDis = (from o in DB.Objective

                                           let oPropos = (           //join และนับค่าที่ต้องการ
                                           from p in DB.Proposition
                                           where p.ObjID == o.ObjID
                                           select o).Count()

                                           select new ObjectiveModel
                                           {
                                               ObjID = o.ObjID,
                                               ObjName = o.ObjName,
                                               TextObj = o.TextObj,
                                               PLessonID = o.LessonID,
                                               CountProposID = oPropos
                                           }).ToList();
                ViewBag.ObjectiveNotDis = ObjectiveDatanotDis;
                var ObjectiveData = (from o in DB.Objective
                                     join l in DB.Lesson on o.LessonID equals l.LessonID
                                     where l.SubjectID == SubjecID
                                     orderby o.ObjID
                                     select new ObjectiveModel
                                     {
                                         ObjID = o.ObjID,
                                         ObjName = o.ObjName
                                     }).ToList();
                ViewBag.PObjective = ObjectiveData;
                var ProposData = (from p in DB.Proposition
                                  join o in DB.Objective on p.ObjID equals o.ObjID
                                  join l in DB.Lesson on o.LessonID equals l.LessonID
                                  // join ps in DB.PropostitionSub on p.ProposID equals ps.ProposID  where p.ProposID == ps.ProposID
                                  where l.SubjectID == SubjecID
                                  orderby p.ProposID
                                  select new PropositionModel
                                  {
                                      ObjID = p.ObjID,
                                      ProposID = p.ProposID,
                                      ProposName = p.ProposName,
                                      TextPropos = p.TextPropos,
                                      ScoreMain = p.ScoreMain,
                                      ObjName = o.ObjName,

                                  }).ToList();
                ViewBag.ProposMain = ProposData;
                var ChoiceData = (from c in DB.Choice
                                  join p in DB.Proposition on c.ProposID equals p.ProposID
                                  where c.ProposID == p.ProposID
                                  orderby c.ProposID
                                  select new ChoiceModel
                                  {
                                      ChoiceID = c.ChoiceID,
                                      ProposID = p.ProposID,
                                      TextChoice = c.TextChoice,
                                      Answer = c.Answer
                                  }).ToList();
                ViewBag.ChoiceShow = ChoiceData;

            }
            return View();
        }
        public ActionResult Propospreview(int ProposID)
        {
            return View();
        }
        public JsonResult CreatePropos(PropositionModel propos)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    if (propos.ProposID == 0) //ในกรณีที่เป็น Auto increment ค่าเริ่มต้นเป็น 0  ถ้า PK ไม่เป็น auto incresment ก็ให้ใช้ != 1 และกำหนด public int isUpdateable { get; set; } ในโมเดล เพื่อเช็ค
                    {
                        Proposition Propos = new Proposition();
                        {
                            Propos.ProposID = propos.ProposID;
                            Propos.ProposName = propos.ProposName;
                            Propos.TextPropos = propos.TextPropos;
                            Propos.CheckChoice = propos.CheckChoice;
                            Propos.ScoreMain = propos.ScoreMain;
                            Propos.ObjID = propos.ObjID;
                            DB.Proposition.Add(Propos);
                            DB.SaveChanges();
                            int indexChoice = propos.Nchoice;
                            Choice pChoice;
                            string[] choice = new string[] { propos.Choice1, propos.Choice2, propos.Choice3, propos.Choice4, propos.Choice5 };
                            double[] answer = new double[] { propos.Answer1, propos.Answer2, propos.Answer3, propos.Answer4, propos.Answer5, };
                            for (int i = 0; i < indexChoice; i++)
                            {
                                pChoice = new Choice();
                                pChoice.ChoiceID = i + 1;
                                pChoice.ProposID = Propos.ProposID;
                                pChoice.TextChoice = choice[i];
                                pChoice.Answer = answer[i];
                                DB.Choice.Add(pChoice);
                                DB.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        var proposUpdate = DB.Proposition.Where(x => x.ProposID == propos.ProposID).FirstOrDefault();
                        if (proposUpdate != null)
                        {
                            DB.Proposition.Where(x => x.ProposID == proposUpdate.ProposID).ForEach(x =>
                            {
                                x.ProposID = propos.ProposID;
                                x.ProposName = propos.ProposName;
                                x.TextPropos = propos.TextPropos;
                                x.ScoreMain = propos.ScoreMain;
                                x.ObjID = propos.ObjID;
                            });
                            DB.SaveChanges();
                        }
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
        public JsonResult EditPropos(int idp)
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
    }
}