using AareonTechnicalTest.EFAudit;
using AareonTechnicalTest.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AareonTechnicalTest
{
    public class ApplicationContext : AuditableContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }

        public virtual DbSet<Person> Persons { get; set; }

        public virtual DbSet<Ticket> Tickets { get; set; }

        public virtual DbSet<TicketNote> TicketNotes { get; set; }

        public string DatabasePath { get; set; }

           protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            PersonConfig.Configure(modelBuilder);
            TicketConfig.Configure(modelBuilder);
            TicketNoteConfig.Configure(modelBuilder);
        }
    }
}
