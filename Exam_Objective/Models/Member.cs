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
    
    public partial class Member
    {
        public int GroupID { get; set; }
        public string UserID { get; set; }
    
        public virtual TestGroup TestGroup { get; set; }
    }
}
