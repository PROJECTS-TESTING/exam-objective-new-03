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
    
    public partial class ExamTopic
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExamTopic()
        {
            this.ExamBodies = new HashSet<ExamBody>();
        }
    
        public int ExamtopicID { get; set; }
        public string ExamtopicName { get; set; }
        public string Explantion { get; set; }
        public string SubjectID { get; set; }
        public string UserID { get; set; }
        public System.DateTime DatetoBegin { get; set; }
        public System.TimeSpan TimetoBegin { get; set; }
        public System.TimeSpan TimetoEnd { get; set; }
        public string Sequences { get; set; }
        public int GroupID { get; set; }
        public string ExamtopicPW { get; set; }
        public Nullable<int> NumberOfTimes { get; set; }
        public string InNetWork { get; set; }
        public string Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamBody> ExamBodies { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
