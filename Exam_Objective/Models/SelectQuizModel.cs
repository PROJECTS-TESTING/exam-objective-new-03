using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exam_Objective.Models
{
    public class SelectQuizModel
    {
        public int ExamBodyID { get; set; }
        public int[] ProposID { get; set; }
    }
    public class ShowQuizModel
    {
        public int ExamBodyID { get; set; }
        public int ProposID { get; set; }
        public string ProposName { get; set; }
        public int LessonID { get; set; }
        public string LesName { get; set; }
    }
}