using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exam_Objective.Models
{
    public class FilexmlModel
    {
        public int ObjID { get; set; }
        public HttpPostedFileBase xmlFile { get; set; }
    }
}