using Microsoft.EntityFrameworkCore;

namespace AareonTechnicalTest.Models
{
    public static class TicketNoteConfig
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketNote>(
                entity =>
                {
                    entity.HasKey(e => e.Id);
                });
        }
    }
}