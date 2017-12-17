using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exam_Objective.Models
{
    public class SubjectsModel
    {
        public string SubjectID { get; set; }
        public string UserID { get; set; }
        public string SubjectName { get; set; }
        public Nullable<int> Theory { get; set; }
        public Nullable<int> Practice { get; set; }
        public string Course { get; set; }
        public string Description { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public int isUpdateable { get; set; }
    }
}