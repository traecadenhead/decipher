﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DecipherEntities : DbContext
    {
        public DecipherEntities()
            : base("name=DecipherEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Demographic> Demographics { get; set; }
        public virtual DbSet<Descriptor> Descriptors { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<PlaceType> PlaceTypes { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<ReviewResponse> ReviewResponses { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Type> Types { get; set; }
        public virtual DbSet<UserDescriptor> UserDescriptors { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ZipDemographic> ZipDemographics { get; set; }
        public virtual DbSet<Zip> Zips { get; set; }
        public virtual DbSet<ZipType> ZipTypes { get; set; }
    }
}
