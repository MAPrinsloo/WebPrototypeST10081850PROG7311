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
    
    public partial class STOCK
    {
        public System.Guid S_ID { get; set; }
        public int WEIGHT { get; set; }
        public System.DateTime DATE_DELIVERED { get; set; }
        public System.Guid P_ID { get; set; }
    
        public virtual PRODUCT PRODUCT { get; set; }
    }
}
