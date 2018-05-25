using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Exam_Objective.Models
{
    [Serializable]
    [XmlRoot("DataImportExport")]
    public class ImportExportModel
    {
        [XmlElement("ProposID")]
        public int ProposID { get; set; }
        [XmlElement("ObjID")]
        public int ObjID { get; set; }
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
        public double ScoreChoice1 { get; set; }
        [XmlElement("Choice2")]
        public string Choice2 { get; set; }
        [XmlElement("ScoreChoice2")]
        public double ScoreChoice2 { get; set; }
        [XmlElement("Choice3")]
        public string Choice3 { get; set; }
        [XmlElement("ScoreChoice3")]
        public double ScoreChoice3 { get; set; }
        [XmlElement("Choice4")]
        public string Choice4 { get; set; }
        [XmlElement("ScoreChoice4")]
        public double ScoreChoice4 { get; set; }
        [XmlElement("Choice5")]
        public string Choice5 { get; set; }
        [XmlElement("ScoreChoice5")]
        public double ScoreChoice5 { get; set; }
    }

}