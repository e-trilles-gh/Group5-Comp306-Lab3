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

        public DbSet<Comment> Comment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Episode>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Episode)
                .WithMany()
                .HasForeignKey(e => e.EpisodeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
