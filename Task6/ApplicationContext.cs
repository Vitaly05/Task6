using Microsoft.EntityFrameworkCore;
using Task6.Models;

namespace Task6
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public ApplicationContext(DbContextOptions builder)
            : base(builder) { }
    }
}
