//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebPrototypeST10081850PROG7311
{
    using System;
    using System.Collections.Generic;
    
    public partial class USER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USER()
        {
            this.FARMs = new HashSet<FARM>();
        }
    
        public System.Guid U_ID { get; set; }
        public string NAME { get; set; }
        public string USERNAME { get; set; }
        public string USER_TYPE { get; set; }
        public System.Guid PWORD_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FARM> FARMs { get; set; }
        public virtual PASSWORD PASSWORD { get; set; }
    }
}
