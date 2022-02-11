using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AareonTechnicalTest.Models
{
    public class TicketNote
    {
        [Key]
        public int Id { get; internal set; }

        public int TicketId { get; internal set; }

        [JsonIgnore]
        public Ticket Ticket { get; set; }

        public int PersonId { get; set; }

        [JsonIgnore]
        public Person Person { get; set; }

        public string NoteContent { get; set; }

        /// <summary>
        /// Requirement: "removed by anyone, but only an Administrator may delete" implies soft delete by user
        /// </summary>
        [JsonIgnore]
        public bool IsRemoved { get; internal set; }
    }
}