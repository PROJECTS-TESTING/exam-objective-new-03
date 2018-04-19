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
    [XmlRoot("Proposition")]
    public class ImportExportModel
    {

        [XmlElement("TextPropos")]
        public string TextPropos { get; set; }
        [XmlElement("ScoreMain")]
        public double ScoreMain { get; set; }
        [XmlElement("CheckChoice")]
        public Nullable<int> CheckChoice { get; set; }
        [XmlElement("Choice1")]
        public string Choice1 { get; set; }
        [XmlElement("Choice2")]
        public string Choice2 { get; set; }
        [XmlElement("Choice3")]
        public string Choice3 { get; set; }
        [XmlElement("Choice4")]
        public string Choice4 { get; set; }
    }

}