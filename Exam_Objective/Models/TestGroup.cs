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
    
    public partial class TestGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TestGroup()
        {
            this.Members = new HashSet<Member>();
        }
    
        public int GroupID { get; set; }
        public string SubjectID { get; set; }
        public string UserID { get; set; }
        public string GroupName { get; set; }
        public string GroupPW { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member> Members { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
