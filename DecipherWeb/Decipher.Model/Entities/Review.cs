//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Decipher.Model.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Review
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Review()
        {
            this.ReviewResponses = new HashSet<ReviewResponse>();
        }
    
        public int ReviewID { get; set; }
        public int UserID { get; set; }
        public string PlaceID { get; set; }
        public Nullable<System.DateTime> VisitDate { get; set; }
        public bool Reported { get; set; }
        public int Score { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateModified { get; set; }
    
        public virtual Place Place { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReviewResponse> ReviewResponses { get; set; }
        public virtual User User { get; set; }
    }
}
