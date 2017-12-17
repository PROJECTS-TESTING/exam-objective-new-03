using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exam_Objective.Models
{
    public class TestGroupModel
    {
        public int GroupID { get; set; }
        public string SubjectID { get; set; }
        public string UserID { get; set; }
        public string StudyGroup { get; set; }
        public string GroupPW { get; set; }
        public int isUpdateable { get; set; }
    }
}