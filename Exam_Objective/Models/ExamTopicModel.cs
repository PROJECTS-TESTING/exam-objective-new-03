using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Exam_Objective.Models
{
    public class ExamTopicModel
    {
        public int ExamtopicID { get; set; }
        public string ExamtopicName { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string Explantion { get; set; }
        public string SubjectID { get; set; }
        public string UserID { get; set; }
        public System.DateTime DatetoBegin { get; set; }
        public System.TimeSpan TimetoBegin { get; set; }
        public System.TimeSpan TimetoEnd { get; set; }
        public int NewPage { get; set; }
        public string HowtoPage { get; set; }
        public string Sequences { get; set; }
        public int GroupID { get; set; }
        public string ExamtopicPW { get; set; }
        public string StudyGroup { get; set; }
        public int NumberOfTimes { get; set; }
        public string InNetWork { get; set; }

        public int isUpdateable { get; set; }
    }
    public class ExamtopicDataModel
    {
        public int ExamtopicID { get; set; }
        public string SubjectID { get; set; }
        public string UserID { get; set; }
        public string ExamtopicPW { get; set; }
        public int CheckDateTime { get; set; }
    }
}