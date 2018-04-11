using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exam_Objective.Models
{
    public class MemberModel
    {
        public int GroupID { get; set; }
        public string UserID { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string StudyGroup { get; set; }
        public int isUpdateable { get; set; }
        public string StudentID { get; set; }
    }

    public class AddGroupStudent
    {
        public string UserID { get; set; }
        public string SubjectID { get; set; }
        public string StudyGroup { get; set; }
        public string GroupPW { get; set; }
    }
}