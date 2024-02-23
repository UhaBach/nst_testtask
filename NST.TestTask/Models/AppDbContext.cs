using Microsoft.EntityFrameworkCore;

namespace NST.TestTask.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Skill> Skills { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
            Database.Migrate();
            Console.WriteLine("app db context created");
        }
    }
}
