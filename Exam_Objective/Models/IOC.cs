//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Exam_Objective.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class IOC
    {
        public int ProposID { get; set; }
        public int IOCScore { get; set; }
        public string UserID { get; set; }
    
        public virtual Proposition Proposition { get; set; }
    }
}
