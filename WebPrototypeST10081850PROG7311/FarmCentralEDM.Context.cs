﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FarmCentralDBEntities : DbContext
    {
        public FarmCentralDBEntities()
            : base("name=FarmCentralDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<FARM> FARMs { get; set; }
        public virtual DbSet<PASSWORD> PASSWORDs { get; set; }
        public virtual DbSet<PRODUCT> PRODUCTs { get; set; }
        public virtual DbSet<STOCK> STOCKs { get; set; }
        public virtual DbSet<USER> USERs { get; set; }
    }
}
