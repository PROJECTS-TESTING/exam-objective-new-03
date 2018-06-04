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
using System.Xml.Linq;
using System.Web.UI.WebControls;

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
                                x.LessonID = lesson.LessonID; //auto increment
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
                                          UserID = le.UserID
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
        public ActionResult DeleteLesson(int id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var DeleteLes = DB.Lesson.Where(x => x.LessonID == id).FirstOrDefault();
                    if (DeleteLes == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        DB.Lesson.Remove(DeleteLes);
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
        //// Update Objective By ObjID
        public JsonResult EditObj(string id)
        {
            var jsonreturn = new JsonRespone();
            int Id = int.Parse(id);
            try
            {
                using (var DB = new dbEntities())
                {
                    var EditObj = (from ob in DB.Objective
                                   join le in DB.Lesson on ob.LessonID equals le.LessonID
                                   where ob.ObjID == Id
                                   select new ObjectiveModel
                                   {
                                       ObjID = ob.ObjID,
                                       ObjName = ob.ObjName,
                                       TextObj = ob.TextObj,
                                       PLessonID = ob.LessonID,
                                       PLesName = le.LesName

                                   }).FirstOrDefault();
                    if (EditObj == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย", data = EditObj };
                    }

                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }
        public ActionResult DeleteObj(int id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var DeleteObj = DB.Objective.Where(x => x.ObjID == id).FirstOrDefault();
                    if (DeleteObj == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        DB.Objective.Remove(DeleteObj);
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
                                      Continuity = p.Continuity

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
                            Propos.Continuity = propos.Continuity;
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
                                x.Continuity = propos.Continuity;
                                x.ObjID = propos.ObjID;
                            });
                            DB.SaveChanges();
                            for (int i = 1; i <= 5; i++)
                            {
                                if (i == 1)
                                {
                                    DB.Choice.Where(c => c.ProposID == propos.ProposID && c.ChoiceID == i).ForEach(c =>
                                    {
                                        c.ChoiceID = i;
                                        c.ProposID = propos.ProposID;
                                        c.TextChoice = propos.Choice1;
                                        c.Answer = propos.Answer1;
                                    });
                                    DB.SaveChanges();
                                }
                                if (i == 2)
                                {
                                    DB.Choice.Where(c => c.ProposID == propos.ProposID && c.ChoiceID == i).ForEach(c =>
                                    {
                                        c.ChoiceID = 2;
                                        c.ProposID = propos.ProposID;
                                        c.TextChoice = propos.Choice2;
                                        c.Answer = propos.Answer2;
                                    });
                                    DB.SaveChanges();
                                }
                                if (i == 3)
                                {
                                    DB.Choice.Where(c => c.ProposID == propos.ProposID && c.ChoiceID == i).ForEach(c =>
                                    {
                                        c.ChoiceID = i;
                                        c.ProposID = propos.ProposID;
                                        c.TextChoice = propos.Choice3;
                                        c.Answer = propos.Answer3;
                                    });
                                    DB.SaveChanges();
                                }
                                if (i == 4)
                                {
                                    DB.Choice.Where(c => c.ProposID == propos.ProposID && c.ChoiceID == i).ForEach(c =>
                                    {
                                        c.ChoiceID = i;
                                        c.ProposID = propos.ProposID;
                                        c.TextChoice = propos.Choice4;
                                        c.Answer = propos.Answer4;
                                    });
                                    DB.SaveChanges();
                                }
                                if (i == 5)
                                {
                                    DB.Choice.Where(c => c.ProposID == propos.ProposID && c.ChoiceID == i).ForEach(c =>
                                    {
                                        c.ChoiceID = i;
                                        c.ProposID = propos.ProposID;
                                        c.TextChoice = propos.Choice5;
                                        c.Answer = propos.Answer5;
                                    });
                                    DB.SaveChanges();
                                }
                            }
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
                    double[] TAnswer = new double[6];
                    for (int i = 1; i <= CountChoice; i++)
                    {
                        TChoice[i] = (from c in DB.Choice
                                      where c.ProposID == idp && c.ChoiceID == i
                                      select c.TextChoice
                                     ).FirstOrDefault();
                        TAnswer[i] = (from c in DB.Choice
                                      where c.ProposID == idp && c.ChoiceID == i
                                      select c.Answer
                                     ).FirstOrDefault();
                    }
                    var TextChoice1 = TChoice[1];
                    var TextChoice2 = TChoice[2];
                    var TextChoice3 = TChoice[3];
                    var TextChoice4 = TChoice[4];
                    var TextChoice5 = TChoice[5];
                    var TextAnswer1 = TAnswer[1];
                    var TextAnswer2 = TAnswer[2];
                    var TextAnswer3 = TAnswer[3];
                    var TextAnswer4 = TAnswer[4];
                    var TextAnswer5 = TAnswer[5];
                    var EditPropos = (from p in DB.Proposition
                                      join c in DB.Proposition on p.ProposID equals c.ProposID
                                      where p.ProposID == idp
                                      select new PropositionModel
                                      {
                                          ProposID = p.ProposID,
                                          ObjID = p.ObjID,
                                          ProposName = p.ProposName,
                                          TextPropos = p.TextPropos,
                                          ScoreMain = p.ScoreMain,
                                          CheckChoice = p.CheckChoice,
                                          Choice1 = TextChoice1,
                                          Choice2 = TextChoice2,
                                          Choice3 = TextChoice3,
                                          Choice4 = TextChoice4,
                                          Choice5 = TextChoice5,
                                          Answer1 = TextAnswer1,
                                          Answer2 = TextAnswer2,
                                          Answer3 = TextAnswer3,
                                          Answer4 = TextAnswer4,
                                          Answer5 = TextAnswer5

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
        //// DeletePropos  By ProposID
        public ActionResult DeletePropos(int id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var DeletePropos = DB.Proposition.Where(x => x.ProposID == id).FirstOrDefault();
                    var ChoiceCount = DB.Choice.Where(c => c.ProposID == id).Count();
                    if (DeletePropos == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        DB.Proposition.Remove(DeletePropos);
                        DB.SaveChanges();
                        for (int i = 1; i <= ChoiceCount; i++)
                        {
                            var DeleteChoice = DB.Choice.Where(d => d.ProposID == id && d.ChoiceID == i).FirstOrDefault();
                            DB.Choice.Remove(DeleteChoice);
                            DB.SaveChanges();
                        }
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
        // GEGIN Import
        // Index of Import
        public ActionResult Import()
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
        [HttpGet]
        public ActionResult ImportMain(string SubjecID)
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
                                      Continuity = p.Continuity

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
        [HttpPost]
        public ActionResult ImportMain(HttpPostedFileBase xmlFile)
        {
            if (xmlFile.ContentType.Equals("application/xml") || xmlFile.ContentType.Equals("text/xml"))
            {
                try
                {
                    var xmlPath = Server.MapPath("~/Content/" + xmlFile.FileName);
                    xmlFile.SaveAs(xmlPath);
                    XDocument xDoc = XDocument.Load(xmlPath);
                    List<Proposition> productList = xDoc.Descendants("PropositionModel").Select(ProductModel => new Proposition
                    {
                        ProposName = ProductModel.Element("ProposName").Value,
                        TextPropos = ProductModel.Element("TextPropos").Value,
                        ScoreMain = Convert.ToDouble(ProductModel.Element("ScoreMain").Value),
                        CheckChoice = Convert.ToInt32(ProductModel.Element("CheckChoice").Value)
                    }).ToList();
                    using (dbEntities de = new dbEntities())
                    {
                        foreach (var i in productList)
                        {
                            var v = de.Proposition.Where(a => a.ProposID.Equals(i.ProposID)).FirstOrDefault();
                            if (v != null)
                            {
                                v.ProposID = i.ProposID;
                                v.ProposName = i.ProposName;
                                v.TextPropos = i.TextPropos;
                                v.ScoreMain = i.ScoreMain;
                                v.CheckChoice = i.CheckChoice;
                            }
                            else
                            {
                                de.Proposition.Add(i);
                            }
                        }
                        de.SaveChanges();
                        ViewBag.Result = de.Proposition.ToList();
                    }
                    return View("ImportMain");
                }
                catch
                {
                    ViewBag.Error = "Can't import xml file";
                    return View("ImportMain");
                }
            }
            else
            {
                ViewBag.Error = "Can't import xml file";
                return View("ImportMain");
            }
        }
        [HttpGet]
        public ActionResult Importin(int ObjID)
        {
            using (var DB = new dbEntities())
            {
                var ObjectiveData = (from o in DB.Objective
                                     where o.ObjID == ObjID
                                     orderby o.ObjID
                                     select new ObjectiveModel
                                     {
                                         ObjID = o.ObjID,
                                         ObjName = o.ObjName
                                     }).ToList();
                ViewBag.Objective = ObjectiveData;
                return View();
            }
        }
        [HttpPost]
        public ActionResult Importin(HttpPostedFileBase xmlFile, int ObjID)
        {
            var jsonreturn = new JsonRespone();
            if (xmlFile != null)
            {
                if (xmlFile.ContentType.Equals("application/xml") || xmlFile.ContentType.Equals("text/xml"))
                {
                    try
                    {
                        var xmlPath = Server.MapPath("~/Content/" + xmlFile.FileName);
                        xmlFile.SaveAs(xmlPath);
                        XDocument xDoc = XDocument.Load(xmlPath);
                        List<ImportExportModel> productList = xDoc.Descendants("DataImportExport").Select(propos => new ImportExportModel
                        {
                            ProposID = 0,
                            ObjID = ObjID,
                            ProposName = propos.Element("ProposName").Value,
                            TextPropos = propos.Element("TextPropos").Value,
                            ScoreMain = Convert.ToInt32(propos.Element("ScoreMain").Value),
                            CheckChoice = Convert.ToInt32(propos.Element("CheckChoice").Value),
                            Choice1 = propos.Element("Choice1").Value,
                            ScoreChoice1 = Convert.ToDouble(propos.Element("ScoreChoice1").Value),
                            Choice2 = propos.Element("Choice2").Value,
                            ScoreChoice2 = Convert.ToDouble(propos.Element("ScoreChoice2").Value),
                            Choice3 = propos.Element("Choice3").Value,
                            ScoreChoice3 = Convert.ToDouble(propos.Element("ScoreChoice3").Value),
                            Choice4 = propos.Element("Choice4").Value,
                            ScoreChoice4 = Convert.ToDouble(propos.Element("ScoreChoice4").Value)
                        }).ToList();
                        using (dbEntities de = new dbEntities())
                        {
                            foreach (var i in productList)
                            {
                                Proposition Propos = new Proposition();
                                {
                                    Propos.ProposID = i.ProposID;
                                    Propos.ObjID = i.ObjID;
                                    Propos.ProposName = i.ProposName;
                                    Propos.TextPropos = i.TextPropos;
                                    Propos.ScoreMain = i.ScoreMain;
                                    Propos.CheckChoice = i.CheckChoice;
                                    de.Proposition.Add(Propos);
                                    de.SaveChanges();
                                    int indexChoice = 4;
                                    Choice pChoice;

                                    string[] choice = new string[] { i.Choice1, i.Choice2, i.Choice3, i.Choice4 };
                                    double[] answer = new double[] { i.ScoreChoice1, i.ScoreChoice2, i.ScoreChoice3, i.ScoreChoice4 };
                                    for (int j = 0; j < indexChoice; j++)
                                    {
                                        pChoice = new Choice();
                                        pChoice.ChoiceID = j + 1;
                                        pChoice.ProposID = Propos.ProposID;
                                        pChoice.TextChoice = choice[j];
                                        pChoice.Answer = answer[j];
                                        de.Choice.Add(pChoice);
                                        de.SaveChanges();
                                    }
                                }
                            }
                        }
                        ViewBag.Success = "นำเข้าข้อสอบสำเร็จ";
                        return View("Success");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "Can't import xml file" + ex;
                        return View("Success");

                    }
                }
                else
                {
                    ViewBag.Error = "Can't import xml file";
                    return View("Success");
                }
            }
            else
            {
                ViewBag.Error = "คุณไม่ได้เลือกไฟล์เพื่อที่จะนำเข้า";
                return View("Success");
            }

        }
        [HttpPost]
        public ActionResult Importin5(HttpPostedFileBase xmlFile, int ObjID)
        {
            var jsonreturn = new JsonRespone();
            if (xmlFile != null)
            {
                if (xmlFile.ContentType.Equals("application/xml") || xmlFile.ContentType.Equals("text/xml"))
                {
                    try
                    {
                        var xmlPath = Server.MapPath("~/Content/" + xmlFile.FileName);
                        xmlFile.SaveAs(xmlPath);
                        XDocument xDoc = XDocument.Load(xmlPath);
                        List<ImportExportModel> productList = xDoc.Descendants("DataImportExport").Select(propos => new ImportExportModel
                        {
                            ProposID = 0,
                            ObjID = ObjID,
                            ProposName = propos.Element("ProposName").Value,
                            TextPropos = propos.Element("TextPropos").Value,
                            ScoreMain = Convert.ToInt32(propos.Element("ScoreMain").Value),
                            CheckChoice = Convert.ToInt32(propos.Element("CheckChoice").Value),
                            Choice1 = propos.Element("Choice1").Value,
                            ScoreChoice1 = Convert.ToDouble(propos.Element("ScoreChoice1").Value),
                            Choice2 = propos.Element("Choice2").Value,
                            ScoreChoice2 = Convert.ToDouble(propos.Element("ScoreChoice2").Value),
                            Choice3 = propos.Element("Choice3").Value,
                            ScoreChoice3 = Convert.ToDouble(propos.Element("ScoreChoice3").Value),
                            Choice4 = propos.Element("Choice4").Value,
                            ScoreChoice4 = Convert.ToDouble(propos.Element("ScoreChoice4").Value),
                            Choice5 = propos.Element("Choice5").Value,
                            ScoreChoice5 = Convert.ToDouble(propos.Element("ScoreChoice5").Value)
                        }).ToList();
                        using (dbEntities de = new dbEntities())
                        {
                            foreach (var i in productList)
                            {
                                Proposition Propos = new Proposition();
                                {
                                    Propos.ProposID = i.ProposID;
                                    Propos.ObjID = i.ObjID;
                                    Propos.ProposName = i.ProposName;
                                    Propos.TextPropos = i.TextPropos;
                                    Propos.ScoreMain = i.ScoreMain;
                                    Propos.CheckChoice = i.CheckChoice;
                                    de.Proposition.Add(Propos);
                                    de.SaveChanges();
                                    int indexChoice = 5;
                                    Choice pChoice;

                                    string[] choice = new string[] { i.Choice1, i.Choice2, i.Choice3, i.Choice4, i.Choice5 };
                                    double[] answer = new double[] { i.ScoreChoice1, i.ScoreChoice2, i.ScoreChoice3, i.ScoreChoice4, i.ScoreChoice5 };
                                    for (int j = 0; j < indexChoice; j++)
                                    {
                                        pChoice = new Choice();
                                        pChoice.ChoiceID = j + 1;
                                        pChoice.ProposID = Propos.ProposID;
                                        pChoice.TextChoice = choice[j];
                                        pChoice.Answer = answer[j];
                                        de.Choice.Add(pChoice);
                                        de.SaveChanges();
                                    }
                                }
                            }
                        }
                        ViewBag.Success = "นำเข้าข้อสอบสำเร็จ";
                        return View("Success");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "Can't import xml file" + ex;
                        return View("Success");

                    }
                }
                else
                {
                    ViewBag.Error = "Can't import xml file";
                    return View("Success");
                }
            }
            else
            {
                ViewBag.Error = "คุณไม่ได้เลือกไฟล์เพื่อที่จะนำเข้า";
                return View("Success");
            }

        }
        [HttpPost]
        public ActionResult Importin2(HttpPostedFileBase xmlFile, int ObjID)
        {
            var jsonreturn = new JsonRespone();
            if (xmlFile != null)
            {
                if (xmlFile.ContentType.Equals("application/xml") || xmlFile.ContentType.Equals("text/xml"))
                {
                    try
                    {
                        var xmlPath = Server.MapPath("~/Content/" + xmlFile.FileName);
                        xmlFile.SaveAs(xmlPath);
                        XDocument xDoc = XDocument.Load(xmlPath);
                        List<ImportExportModel> productList = xDoc.Descendants("DataImportExport").Select(propos => new ImportExportModel
                        {
                            ProposID = 0,
                            ObjID = ObjID,
                            ProposName = propos.Element("ProposName").Value,
                            TextPropos = propos.Element("TextPropos").Value,
                            ScoreMain = Convert.ToInt32(propos.Element("ScoreMain").Value),
                            CheckChoice = Convert.ToInt32(propos.Element("CheckChoice").Value),
                            Choice1 = propos.Element("Choice1").Value,
                            ScoreChoice1 = Convert.ToDouble(propos.Element("ScoreChoice1").Value),
                            Choice2 = propos.Element("Choice2").Value,
                            ScoreChoice2 = Convert.ToDouble(propos.Element("ScoreChoice2").Value)
                        }).ToList();
                        using (dbEntities de = new dbEntities())
                        {
                            foreach (var i in productList)
                            {
                                Proposition Propos = new Proposition();
                                {
                                    Propos.ProposID = i.ProposID;
                                    Propos.ObjID = i.ObjID;
                                    Propos.ProposName = i.ProposName;
                                    Propos.TextPropos = i.TextPropos;
                                    Propos.ScoreMain = i.ScoreMain;
                                    Propos.CheckChoice = i.CheckChoice;
                                    de.Proposition.Add(Propos);
                                    de.SaveChanges();
                                    int indexChoice = 2;
                                    Choice pChoice;

                                    string[] choice = new string[] { i.Choice1, i.Choice2 };
                                    double[] answer = new double[] { i.ScoreChoice1, i.ScoreChoice2 };
                                    for (int j = 0; j < indexChoice; j++)
                                    {
                                        pChoice = new Choice();
                                        pChoice.ChoiceID = j + 1;
                                        pChoice.ProposID = Propos.ProposID;
                                        pChoice.TextChoice = choice[j];
                                        pChoice.Answer = answer[j];
                                        de.Choice.Add(pChoice);
                                        de.SaveChanges();
                                    }
                                }
                            }
                        }
                        ViewBag.Success = "นำเข้าข้อสอบสำเร็จ";
                        return View("Success");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "Can't import xml file" + ex;
                        return View("Success");

                    }
                }
                else
                {
                    ViewBag.Error = "Can't import xml file";
                    return View("Success");
                }
            }
            else
            {
                ViewBag.Error = "คุณไม่ได้เลือกไฟล์เพื่อที่จะนำเข้า";
                return View("Success");
            }

        }
        // Import Proposition Gift Format
        public ActionResult ImportinGift(HttpPostedFileBase postedFile, int ObjID) 
        {
            var jsonreturn = new JsonRespone();
            if (postedFile != null)
            {
                try
                {
                    string filePath = string.Empty;
                    if (postedFile != null)
                    {
                        string path = Server.MapPath("~/Uploads/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        filePath = path + Path.GetFileName(postedFile.FileName);
                        string extension = Path.GetExtension(postedFile.FileName);
                        postedFile.SaveAs(filePath);

                        //Create a DataTable.
                        DataTable dt = new DataTable();
                        dt.Columns.AddRange(new DataColumn[14] { new DataColumn("ProposID", typeof(int)),
                                new DataColumn("ObjID", typeof(int)),
                                new DataColumn("ProposName", typeof(string)),
                                new DataColumn("TextPropos", typeof(string)),
                                new DataColumn("ScoreMain", typeof(int)),
                                new DataColumn("CheckChoice", typeof(int)),
                                new DataColumn("Choice1", typeof(string)),
                                new DataColumn("ScoreChoice1", typeof(double)),
                                new DataColumn("Choice2", typeof(string)),
                                new DataColumn("ScoreChoice2", typeof(double)),
                                new DataColumn("Choice3", typeof(string)),
                                new DataColumn("ScoreChoice3", typeof(double)),
                                new DataColumn("Choice4", typeof(string)),
                                new DataColumn("ScoreChoice4", typeof(double))});
                        //Read the contents of CSV file.
                        string csvData = System.IO.File.ReadAllText(filePath);

                        //Execute a loop over the rows.
                        foreach (string row in csvData.Split('}'))
                        {
                            if (!string.IsNullOrEmpty(row) && row.Length > 10)
                            {
                                dt.Rows.Add();
                                int i = 0;
                                int a = 0;
                                int[] ans = new int[4];
                                // Using for loop reading from array
                                for (int j = 0; j < row.Length; j++)
                                {
                                    if (row[j] == '=' || row[j] == '~')
                                    {
                                        if (row[j] == '=')
                                        {
                                            ans[a] = 1;
                                        }
                                        else
                                        {
                                            ans[a] = 0;
                                        }
                                        a++;
                                    }
                                }
                                //Execute a loop over the columns.
                                a = 0;
                                foreach (string cell in row.Split('=', '~'))
                                {
                                    if (i == 0)
                                    {
                                        string[] cell1 = cell.Split('{');
                                        dt.Rows[dt.Rows.Count - 1][i] = 0; i++;
                                        dt.Rows[dt.Rows.Count - 1][i] = ObjID; i++;
                                        dt.Rows[dt.Rows.Count - 1][i] = cell1[0]; i++;
                                        dt.Rows[dt.Rows.Count - 1][i] = cell1[0]; i++;
                                        dt.Rows[dt.Rows.Count - 1][i] = 1; i++;
                                        dt.Rows[dt.Rows.Count - 1][i] = 1;
                                    }
                                    else
                                    {
                                        dt.Rows[dt.Rows.Count - 1][i] = cell; i++;
                                        dt.Rows[dt.Rows.Count - 1][i] = ans[a]; a++;
                                    }
                                    i++;
                                }
                            }
                        }
                        List<ImportExportModel> dataImport = new List<ImportExportModel>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ImportExportModel import = new ImportExportModel();
                            import.ProposID = Convert.ToInt32(dt.Rows[i]["ProposID"]);
                            import.ObjID = Convert.ToInt32(dt.Rows[i]["ObjID"]);
                            import.ProposName = dt.Rows[i]["ProposName"].ToString();
                            import.TextPropos = dt.Rows[i]["TextPropos"].ToString();
                            import.ScoreMain = Convert.ToInt32(dt.Rows[i]["ScoreMain"]);
                            import.CheckChoice = Convert.ToInt32(dt.Rows[i]["CheckChoice"]);
                            import.Choice1 = dt.Rows[i]["Choice1"].ToString();
                            import.ScoreChoice1 = Convert.ToDouble(dt.Rows[i]["ScoreChoice1"]);
                            import.Choice2 = dt.Rows[i]["Choice2"].ToString();
                            import.ScoreChoice2 = Convert.ToDouble(dt.Rows[i]["ScoreChoice2"]);
                            import.Choice3 = dt.Rows[i]["Choice3"].ToString();
                            import.ScoreChoice3 = Convert.ToDouble(dt.Rows[i]["ScoreChoice3"]);
                            import.Choice4 = dt.Rows[i]["Choice4"].ToString();
                            import.ScoreChoice4 = Convert.ToDouble(dt.Rows[i]["ScoreChoice4"]);
                            dataImport.Add(import);
                        }
                        int Repeat = 0;
                        using (var DB = new dbEntities())
                        {
                            var ProposData = (from p in DB.Proposition
                                              where p.ObjID == ObjID
                                              orderby p.ProposID
                                              select new PropositionModel
                                              {
                                                  TextPropos = p.TextPropos,
                                              }).ToList();
                            foreach (var i in dataImport)
                            {
                                foreach (var j in ProposData)
                                {
                                    if (i.TextPropos == j.TextPropos)
                                    {
                                        Repeat = 1;
                                    }
                                }
                            }
                        }
                        if (Repeat == 0)
                        {
                            using (dbEntities de = new dbEntities())
                            {
                                foreach (var i in dataImport)
                                {
                                    Proposition Propos = new Proposition();
                                    {
                                        Propos.ProposID = i.ProposID;
                                        Propos.ObjID = i.ObjID;
                                        Propos.ProposName = i.ProposName;
                                        Propos.TextPropos = i.TextPropos;
                                        Propos.ScoreMain = i.ScoreMain;
                                        Propos.CheckChoice = i.CheckChoice;
                                        de.Proposition.Add(Propos);
                                        de.SaveChanges();
                                        int indexChoice = 4;
                                        Choice pChoice;

                                        string[] choice = new string[] { i.Choice1, i.Choice2, i.Choice3, i.Choice4 };
                                        double[] answer = new double[] { i.ScoreChoice1, i.ScoreChoice2, i.ScoreChoice3, i.ScoreChoice4 };
                                        for (int j = 0; j < indexChoice; j++)
                                        {
                                            pChoice = new Choice();
                                            pChoice.ChoiceID = j + 1;
                                            pChoice.ProposID = Propos.ProposID;
                                            pChoice.TextChoice = choice[j];
                                            pChoice.Answer = answer[j];
                                            de.Choice.Add(pChoice);
                                            de.SaveChanges();
                                        }
                                    }
                                }
                            }
                            ViewBag.Success = "นำเข้าข้อสอบสำเร็จ";
                            
                        } else {
                            ViewBag.Success = "นำเข้าข้อสอบไม่สำเร็จมีข้อสอบซ้ำ"; 
                        }
                        
                    }
                    return View("Success");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Can't import xml file" + ex;
                    return View("Success");

                }
            }
            else
            {
                ViewBag.Error = "คุณไม่ได้เลือกไฟล์เพื่อที่จะนำเข้า";
                return View("Success");
            }

        }
        // GEGIN Export
        // Index of Export
        public ActionResult Export()
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
        public ActionResult ExportMain(string SubjecID)
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
                                      Continuity = p.Continuity

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
        public void ExportFile(int ObjID)  //ExportFile XML
        {
            var ObjName = "";
            string[] TChoice = new string[6];
            using (var DB = new dbEntities())
            {
                ObjName = (from o in DB.Objective where o.ObjID == ObjID select o.ObjName).FirstOrDefault();

            }

            using (var DB = new dbEntities())
            {

                var dataPropos = (from p in DB.Proposition
                                      //join c in DB.Choice on p.ProposID equals c.ProposID
                                  where p.ObjID == ObjID
                                  orderby p.ProposID
                                  select new IXPropositionModel
                                  {
                                      ProposID = p.ProposID,
                                      ProposName = p.ProposName,
                                      TextPropos = p.TextPropos,
                                      ScoreMain = p.ScoreMain,
                                      CheckChoice = p.CheckChoice
                                  }).ToList();
                var dataChoice = (from c in DB.Choice
                                  join p in DB.Proposition on c.ProposID equals p.ProposID
                                  where p.ObjID == ObjID
                                  orderby p.ProposID
                                  select new IXChoiceModel
                                  {
                                      ChoiceID = c.ChoiceID,
                                      ProposID = c.ProposID,
                                      TextChoice = c.TextChoice,
                                      Answer = c.Answer
                                  }).ToList();
                List<DataImportExport> dataExprot = new List<DataImportExport>();
                string[] Choice = new string[6];
                for (var i = 0; i < dataPropos.Count; i++)
                {
                    dataExprot.Add(new DataImportExport
                    {
                        //ProposID = dataPropos[i].ProposID,
                        ProposName = dataPropos[i].ProposName,
                        TextPropos = dataPropos[i].TextPropos,
                        ScoreMain = dataPropos[i].ScoreMain,
                        CheckChoice = dataPropos[i].CheckChoice,
                        Choice1 = "",
                        Choice2 = "",
                        Choice3 = "",
                        Choice4 = "",
                        Answer1 = 0.0f,
                        Answer2 = 0.0f,
                        Answer3 = 0.0f,
                        Answer4 = 0.0f,
                    });
                    foreach (var dataCo in dataChoice)
                    {
                        if (dataPropos[i].ProposID == dataCo.ProposID)
                        {
                            if (dataCo.ChoiceID == 1)
                            {
                                dataExprot.ElementAt(i).Choice1 = dataCo.TextChoice;
                                dataExprot.ElementAt(i).Answer1 = dataCo.Answer;
                            }
                            if (dataCo.ChoiceID == 2)
                            {
                                dataExprot.ElementAt(i).Choice2 = dataCo.TextChoice;
                                dataExprot.ElementAt(i).Answer2 = dataCo.Answer;
                            }
                            if (dataCo.ChoiceID == 3)
                            {
                                dataExprot.ElementAt(i).Choice3 = dataCo.TextChoice;
                                dataExprot.ElementAt(i).Answer3 = dataCo.Answer;
                            }
                            if (dataCo.ChoiceID == 4)
                            {
                                dataExprot.ElementAt(i).Choice4 = dataCo.TextChoice;
                                dataExprot.ElementAt(i).Answer4 = dataCo.Answer;
                            }
                        }
                    }
                }
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename = " + ObjName + ".xml");
                Response.ContentType = "text/xml";
                var serializer = new System.Xml.Serialization.XmlSerializer(dataExprot.GetType());
                serializer.Serialize(Response.OutputStream, dataExprot);
            }
        }
        public void ExportFileGift(int ObjID)  //Export GIFT Format
        {
            StringWriter sw = new StringWriter();

            
            var ObjName = "";
            string[] TChoice = new string[6];
            using (var DB = new dbEntities())
            {
                ObjName = (from o in DB.Objective where o.ObjID == ObjID select o.ObjName).FirstOrDefault();

            }

            using (var DB = new dbEntities())
            {

                var dataPropos = (from p in DB.Proposition
                                      //join c in DB.Choice on p.ProposID equals c.ProposID
                                  where p.ObjID == ObjID
                                  orderby p.ProposID
                                  select new IXPropositionModel
                                  {
                                      ProposID = p.ProposID,
                                      ProposName = p.ProposName,
                                      TextPropos = p.TextPropos,
                                      ScoreMain = p.ScoreMain,
                                      CheckChoice = p.CheckChoice
                                  }).ToList();
                var dataChoice = (from c in DB.Choice
                                  join p in DB.Proposition on c.ProposID equals p.ProposID
                                  where p.ObjID == ObjID
                                  orderby p.ProposID
                                  select new IXChoiceModel
                                  {
                                      ChoiceID = c.ChoiceID,
                                      ProposID = c.ProposID,
                                      TextChoice = c.TextChoice,
                                      Answer = c.Answer
                                  }).ToList();
                List<DataImportExport> dataExprot = new List<DataImportExport>();
                string[] Choice = new string[6];
                for (var i = 0; i < dataPropos.Count; i++)
                {
                    dataExprot.Add(new DataImportExport
                    {
                        //ProposID = dataPropos[i].ProposID,
                        ProposName = dataPropos[i].ProposName,
                        TextPropos = dataPropos[i].TextPropos,
                        ScoreMain = dataPropos[i].ScoreMain,
                        CheckChoice = dataPropos[i].CheckChoice,
                        Choice1 = "",
                        Choice2 = "",
                        Choice3 = "",
                        Choice4 = "",
                        Answer1 = 0.0f,
                        Answer2 = 0.0f,
                        Answer3 = 0.0f,
                        Answer4 = 0.0f,
                        AnsQ1 = "",
                        AnsQ2 = "",
                        AnsQ3 = "",
                        AnsQ4 = "",
                    });
                    foreach (var dataCo in dataChoice)
                    {
                        if (dataPropos[i].ProposID == dataCo.ProposID)
                        {
                            if (dataCo.ChoiceID == 1)
                            {
                                dataExprot.ElementAt(i).Choice1 = dataCo.TextChoice;
                                dataExprot.ElementAt(i).Answer1 = dataCo.Answer;
                                if (dataCo.Answer > 0){
                                    dataExprot.ElementAt(i).AnsQ1 = "=";
                                }
                                else{
                                    dataExprot.ElementAt(i).AnsQ1 = "~";
                                }
                            }
                            if (dataCo.ChoiceID == 2)
                            {
                                dataExprot.ElementAt(i).Choice2 = dataCo.TextChoice;
                                dataExprot.ElementAt(i).Answer2 = dataCo.Answer;
                                if (dataCo.Answer > 0)
                                {
                                    dataExprot.ElementAt(i).AnsQ2 = "=";
                                }
                                else
                                {
                                    dataExprot.ElementAt(i).AnsQ2 = "~";
                                }
                            }
                            if (dataCo.ChoiceID == 3)
                            {
                                dataExprot.ElementAt(i).Choice3 = dataCo.TextChoice;
                                dataExprot.ElementAt(i).Answer3 = dataCo.Answer;
                                if (dataCo.Answer > 0)
                                {
                                    dataExprot.ElementAt(i).AnsQ3 = "=";
                                }
                                else
                                {
                                    dataExprot.ElementAt(i).AnsQ3 = "~";
                                }
                            }
                            if (dataCo.ChoiceID == 4)
                            {
                                dataExprot.ElementAt(i).Choice4 = dataCo.TextChoice;
                                dataExprot.ElementAt(i).Answer4 = dataCo.Answer;
                                if (dataCo.Answer > 0)
                                {
                                    dataExprot.ElementAt(i).AnsQ4 = "=";
                                }
                                else
                                {
                                    dataExprot.ElementAt(i).AnsQ4 = "~";
                                }
                            }
                        }
                    }
                }                
                foreach (var propos in dataExprot)
                {
                    sw.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}",
                        propos.TextPropos,
                        "{",
                        propos.AnsQ1,
                        propos.Choice1,
                        propos.AnsQ2,
                        propos.Choice2,
                        propos.AnsQ3,
                        propos.Choice3,
                        propos.AnsQ4,
                        propos.Choice4,
                        "}"));
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename = " + ObjName + ".txt");
                Response.ContentType = "text/plain";
                Response.Write(sw.ToString());
                Response.End();            }

        }
    }
}