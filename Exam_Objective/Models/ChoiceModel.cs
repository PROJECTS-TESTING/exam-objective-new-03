using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Exam_Objective.Models
{
    public class ChoiceModel
    {
        public int ChoiceID { get; set; }
        public int ProposID { get; set; }
        public string TextChoice { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public double Answer { get; set; }
        public Nullable<double> Distracter { get; set; }
        public int countAnswer { get; set; }
    }
}