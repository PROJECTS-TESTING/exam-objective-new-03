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
    public class IOCController : Controller
    {
        // GET: IOC
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
                ViewBag.SubjectIOC = SubjectData;
            }
            return View();
        }

        public ActionResult AddGroupIOC(string etid)
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
                                   where u.UserID == user.UserID && s.SubjectID == etid
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
            return View();
        }

        public JsonResult CreateGroup(TestGroupModel group)
        {
            var jsonretern = new JsonRespone();
            var user = Session["User"] as UserSystemModel;
            group.UserID = user.UserID;
            var checkG = group.StudyGroup;
            try
            {
                using (var DB = new dbEntities())
                {
                    if (group.isUpdateable != 1)
                    {
                        var checkGruop = (from g in DB.TestGroup where g.SubjectID == @group.SubjectID && g.UserID == user.UserID && g.GroupName == @group.StudyGroup select g.GroupName).FirstOrDefault();

                        if (!group.StudyGroup.Equals(checkGruop))
                        {
                            group.GroupID = DB.TestGroup.Max(x => x.GroupID) + 1;
                            DB.TestGroup.Add(new TestGroup
                            {
                                GroupID = group.GroupID,
                                SubjectID = group.SubjectID,
                                UserID = group.UserID,
                                GroupName = group.StudyGroup,
                                GroupPW = group.GroupPW,

                            });
                            DB.SaveChanges();
                            checkG = "";
                        }
                        else { checkG = "มีกลุ่มเรียนนี้ในวิชาแล้ว"; }
                    }
                    else
                    {
                        var GroupUpdate = DB.TestGroup.Where(x => x.GroupID == group.GroupID).FirstOrDefault();
                        if (GroupUpdate != null)
                        {
                            DB.TestGroup.Where(x => x.GroupID == GroupUpdate.GroupID).ForEach(x =>
                            {
                                x.GroupName = group.StudyGroup;
                                x.GroupPW = group.GroupPW;
                            });
                            DB.SaveChanges();
                        }
                    }
                    if (checkG.Equals(""))
                    {
                        jsonretern = new JsonRespone { status = true, message = "บันทึกเรียบร้อย" };
                    }
                    else
                    {
                        jsonretern = new JsonRespone { status = true, message = checkG };
                    }
                }
            }
            catch (Exception ex)
            {
                jsonretern = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }

            return Json(jsonretern);
        }

        public JsonResult EditGroup(int id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var EditG = (from s in DB.TestGroup
                                 where s.GroupID == id
                                 select new TestGroupModel
                                 {
                                     StudyGroup = s.GroupName,
                                     GroupPW = s.GroupPW

                                 }).FirstOrDefault();
                    if (EditG == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        jsonreturn = new JsonRespone { status = true, message = "บันทึกเรียบร้อย", data = EditG };
                    }
                }
            }
            catch (Exception ex)
            {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }
        // Delete
        public JsonResult DeleteGroup(int id)
        {
            var jsonreturn = new JsonRespone();
            try
            {
                using (var DB = new dbEntities())
                {
                    var Deletegroup = DB.TestGroup.Where(x => x.GroupID == id).FirstOrDefault();
                    if (Deletegroup == null)
                    {
                        jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" };
                    }
                    else
                    {
                        DB.TestGroup.Remove(Deletegroup);
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

        public ActionResult Contents(string SubjecID)
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
            }
            return View();
        }

        public ActionResult SpecialistLogin()
        {
            return View();
        }

        public ActionResult Specialist(string SubjecID ,string subid,string etid)
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
                var ObjectiveDatanotDis = (from l in DB.Lesson
                                           join o in DB.Objective on l.LessonID equals o.LessonID
                                           join p in DB.Proposition on o.ObjID equals p.ObjID
                                           where l.SubjectID == SubjecID
                                           //ยังเอามาทุกข้อ
                                           select new ObjectiveModel
                                           {
                                               ObjID = o.ObjID,
                                               ObjName = o.ObjName,
                                               TextObj = o.TextObj,
                                               PLessonID = o.LessonID,
                                               //CountProposID = oPropos
                                           }).ToList();
                ViewBag.ObjectiveNotDis = ObjectiveDatanotDis;
            }
            using (var DB = new dbEntities())
            {
                var LessonData = (from l in DB.Lesson
                                  where user.UserID == l.UserID && l.SubjectID == etid
                                  orderby l.LessonID
                                  select new LessonModel
                                  {
                                      LessonID = l.LessonID,
                                      LesName = l.LesName,
                                      TextLesson = l.TextLesson

                                  }).DistinctBy(r => r.LesName).ToList();
                ViewBag.DLesson = LessonData;
            }
            using (var DB = new dbEntities())
            {
                var dataProp = (from l in DB.Lesson
                                join o in DB.Objective on l.LessonID equals o.LessonID
                                join p in DB.Proposition on o.ObjID equals p.ObjID
                                where l.SubjectID == SubjecID
                                select new PropositionModel
                                {
                                    ProposID = p.ProposID,
                                    ProposName = p.ProposName,
                                    TextPropos = p.TextPropos,
                                    Continuity = p.Continuity
                                }).ToList();
                ViewBag.dataProposition = dataProp;
            }
            using (var DB = new dbEntities())
            {
                ViewBag.dataChoice = (from l in DB.Lesson
                                      join o in DB.Objective on l.LessonID equals o.LessonID
                                      join p in DB.Proposition on o.ObjID equals p.ObjID
                                      join c in DB.Choice on p.ProposID equals c.ProposID
                                      where l.SubjectID == SubjecID
                                      select new ChoiceModel
                                      {
                                          ProposID = p.ProposID,
                                          ChoiceID = c.ChoiceID,
                                          TextChoice = c.TextChoice,
                                          Answer = c.Answer,
                                      }).ToList();
            }
            using (var DB = new dbEntities())
            {
                ViewBag.contens = (from l in DB.Lesson
                                   where l.SubjectID == SubjecID
                                   select new LessonModel
                                   {
                                       LessonID = l.LessonID,
                                       Contents = l.Contents
                                   }
                    ).ToList();
            }
            return View();
        }

        public ActionResult indexGroup()
        {   //ยัง
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
                ViewBag.SubjectIOC = SubjectData;
            }

            return View();
        }

        /*public JsonResult ViewPropos(int idp)
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
        }*/
    }
}