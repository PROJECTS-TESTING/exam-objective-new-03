using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Exam_Objective.Models
{
    public class IXChoiceModel
    {
        [XmlElement("ChoiceID")]
        public int ChoiceID { get; set; }
        [XmlElement("ProposID")]
        public int ProposID { get; set; }
        [XmlElement("TextChoice")]
        public string TextChoice { get; set; }
        [XmlElement("Answer")]
        public double Answer { get; set; }
    }
}