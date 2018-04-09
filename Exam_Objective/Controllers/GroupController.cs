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
    public class GroupController : Controller
    {
        // GET: Group
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
                                       Theory = s.Theory,
                                       Practice = s.Practice,
                                       Course = s.Course,
                                       UserID = s.UserID
                                   }).ToList();
                ViewBag.Subject = SubjectData;
            }
            return View();
        }

        public ActionResult AddGroup(string etid)
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
            using (var DB = new dbEntities()) {
               
                var GroupData = (from g in DB.TestGroup
                                 where g.UserID == user.UserID && g.SubjectID == etid
                                 select new TestGroupModel {
                                     GroupID = g.GroupID,
                                     StudyGroup = g.GroupName,
                                     GroupPW = g.GroupPW
                                 } ).ToList();
                ViewBag.testGroup = GroupData;
            }
                return View();
        }

        public JsonResult CreateGroup(TestGroupModel group) {
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
                            group.GroupID = (DB.TestGroup.Count()!=0) ? DB.TestGroup.Max(x => x.GroupID) + 1 : 1;
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
                using(var DB = new dbEntities())
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
            catch (Exception ex) {
                jsonreturn = new JsonRespone { status = false, message = "เกิดข้อผิดพลาด" + ex.Message };
            }
            return Json(jsonreturn);
        }
        // Delete
        public JsonResult DeleteGroup(int id) {
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
        // สมาชิกของกลุ่ม
        public ActionResult ViewMemder(int etid)
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
            using (var DB = new dbEntities()) {
                var DataMember = (from u in DB.UserSystem
                                  join g in DB.Member on u.UserID equals g.UserID
                                  join m in DB.TestGroup on g.GroupID equals m.GroupID
                                  where g.GroupID == etid
                                  select new MemberModel
                                  {
                                      StudentID = u.StudentID,
                                      Fname = u.Fname,
                                      Lname = u.Lname,
                                      StudyGroup = m.GroupName
                                  }).ToList();
                ViewBag.DataMem = DataMember;
            }
            using (var DB = new dbEntities()) {
                var pathSuj = (from g in DB.TestGroup
                               join s in DB.Subjects on g.SubjectID equals s.SubjectID
                               where g.GroupID == etid
                               select new PathMemberModel {
                                   StudyGroup = g.GroupName,
                                   SubjectName = s.SubjectName
                               }).ToList();
                ViewBag.pathSujG = pathSuj;
            }
            return View();
        }
    }
}