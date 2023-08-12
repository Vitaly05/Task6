using System.ComponentModel.DataAnnotations;

namespace Task6.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string Data { get; set; } = "";

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
