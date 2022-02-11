using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AareonTechnicalTest.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; internal set; }

        public string Content { get; set; }

        public int PersonId { get; set; }
        
        [JsonIgnore]
        public Person Person { get; set; }
    }
}
