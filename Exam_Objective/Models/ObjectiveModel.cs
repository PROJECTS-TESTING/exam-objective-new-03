using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Exam_Objective.Models
{
    public class ObjectiveModel
    {
        public int ObjID { get; set; }
        public string ObjName { get; set; }
        public string TextObj { get; set; }
        public int PLessonID { get; set; }
        public int CountProposID { get; set; }
    }
}