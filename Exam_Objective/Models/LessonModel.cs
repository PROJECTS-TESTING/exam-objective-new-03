using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Exam_Objective.Models
{
    public class LessonModel
    {
        public int LessonID { get; set; }
        public string LesName { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string TextLesson { get; set; }
        public string SubjectID { get; set; }
        public string UserID { get; set; }
        public string SubjectName { get; set; }  //สร้างไว้สำหรับ join
        public string Fname { get; set; }  //สร้างไว้สำหรับ join
<<<<<<< HEAD
        public int CountProposID { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string Contents { get; set; }
=======
        public int CountObjective { get; set; }
        public int Content { get; set; }
>>>>>>> 4543c8e40192342d8133acccf0d7f6897431f5a0
    }
}