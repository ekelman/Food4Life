﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Food4Life.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class superhealthyeatsEntities : DbContext
    {
        public superhealthyeatsEntities()
            : base("name=superhealthyeatsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AboutPage> AboutPages { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
    }
}
