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
=======
        public int CountObjective { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string Contents { get; set; }
>>>>>>> 0f567a1f496edc3c2714b8e2ec92ee16e35552a2
    }
}