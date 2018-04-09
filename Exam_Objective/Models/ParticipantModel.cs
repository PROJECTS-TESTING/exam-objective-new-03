using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exam_Objective.Models
{
    public class ParticipantModel
    {
        public int isUpdateable { get; set; }
        public string ParticipantID { get; set; }
        public string PUserID { get; set; }
        public string PSubjectID { get; set; }
        public string PStatus { get; set; }
        public string PFname { get; set; }

        public virtual Subject Subjects { get; set; }
        public virtual UserSystem UserSystem { get; set; }
    }
}