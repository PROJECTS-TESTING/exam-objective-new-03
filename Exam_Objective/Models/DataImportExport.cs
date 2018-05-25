using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Exam_Objective.Models
{
    public class DataImportExport
    {
        //[XmlElement("ProposID")]
        //public int ProposID { get; set; }
        [XmlElement("ProposName")]
        public string ProposName { get; set; }
        [XmlElement("TextPropos")]
        public string TextPropos { get; set; }
        [XmlElement("ScoreMain")]
        public double ScoreMain { get; set; }
        [XmlElement("CheckChoice")]
        public Nullable<int> CheckChoice { get; set; }
        [XmlElement("Choice1")]
        public string Choice1 { get; set; }
        [XmlElement("ScoreChoice1")]
        public double Answer1 { get; set; }
        [XmlElement("Choice2")]
        public string Choice2 { get; set; }
        [XmlElement("ScoreChoice2")]
        public double Answer2 { get; set; }
        [XmlElement("Choice3")]
        public string Choice3 { get; set; }
        [XmlElement("ScoreChoice3")]
        public double Answer3 { get; set; }
        [XmlElement("Choice4")]
        public string Choice4 { get; set; }
        [XmlElement("ScoreChoice4")]
        public double Answer4 { get; set; }
    }
}