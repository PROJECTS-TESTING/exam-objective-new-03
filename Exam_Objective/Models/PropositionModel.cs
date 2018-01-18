using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Exam_Objective.Models
{
    public class PropositionModel
    {
       
        public int ProposID { get; set; }
        public int ObjID { get; set; }
        public string ProposName { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string TextPropos { get; set; }
        public double ScoreMain { get; set; }
        public Nullable<int> Continuity { get; set; }
        public Nullable<double> Difficulty { get; set; }
        public Nullable<double> Discimination { get; set; }
        public Nullable<double> IOC { get; set; }
        public Nullable<int> CheckChoice { get; set; }
        //for join
        public string ObjName { get; set; }
        public int Nchoice { get; set; }
        public int LessonID { get; set; }
        public string LesName { get; set; }
        //for Choice        
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string Choice1 { get; set; }
        public double Answer1 { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string Choice2 { get; set; }
        public double Answer2 { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string Choice3 { get; set; }
        public double Answer3 { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string Choice4 { get; set; }
        public double Answer4 { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string Choice5 { get; set; }
        public double Answer5 { get; set; }
    }

}