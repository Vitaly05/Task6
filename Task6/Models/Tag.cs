using System.ComponentModel.DataAnnotations;

namespace Task6.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
