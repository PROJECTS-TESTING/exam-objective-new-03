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
    
    public partial class Proposition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Proposition()
        {
            this.Choices = new HashSet<Choice>();
            this.GetExams = new HashSet<GetExam>();
        }
    
        public int ProposID { get; set; }
        public int ObjID { get; set; }
        public string ProposName { get; set; }
        public string TextPropos { get; set; }
        public double ScoreMain { get; set; }
        public Nullable<int> Continuity { get; set; }
        public Nullable<double> Difficulty { get; set; }
        public Nullable<double> Discimination { get; set; }
        public Nullable<double> IOC { get; set; }
        public Nullable<int> CheckChoice { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Choice> Choices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GetExam> GetExams { get; set; }
        public virtual IOC IOC1 { get; set; }
        public virtual Objective Objective { get; set; }
    }
}
