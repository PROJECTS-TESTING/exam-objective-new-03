using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Exam_Objective.Models
{
    [Serializable]
    [XmlRoot("Proposition")]
    public class IXPropositionModel
    {
        [XmlElement("ProposID")]
        public int ProposID { get; set; }
        [XmlElement("ProposName")]
        public string ProposName { get; set; }
        [XmlElement("TextPropos")]
        public string TextPropos { get; set; }
        [XmlElement("ScoreMain")]
        public double ScoreMain { get; set; }
        [XmlElement("CheckChoice")]
        public Nullable<int> CheckChoice { get; set; }
    }
}