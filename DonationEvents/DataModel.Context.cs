﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DonationEvents
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EventsMasterEntities : DbContext
    {
        public EventsMasterEntities()
            : base("name=EventsMasterEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DonationRange> DonationRanges { get; set; }
        public virtual DbSet<UserDonation> UserDonations { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<ViewedEvent> ViewedEvents { get; set; }
    }
}
