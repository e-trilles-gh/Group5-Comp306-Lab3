using Microsoft.EntityFrameworkCore;

namespace group_5_Mohti_Trilles__Lab_3.Model
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }

        public DbSet<Episode> Episode { get; set; }
    }
}
