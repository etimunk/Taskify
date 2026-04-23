using Microsoft.EntityFrameworkCore;
using Taskify.Core.Entities;

namespace Taskify.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Project> projects { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Tasks> tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=Taskify_DB");
            optionsBuilder.LogTo(m => Console.WriteLine(m));  // להדפיס לוגים לצורך Debug
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // קשר בין Tasks ל-User
            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.User)  // קשר ל-User
                .WithMany(u => u.Tasks)  // כל משתמש יכול לבצע מספר משימות
                .HasForeignKey(t => t.UserId)  // המפתח הזר כאן הוא UserId
                .OnDelete(DeleteBehavior.Restrict);  // מונע מחיקה אוטומטית של משימות אם המשתמש נמחק

            // קשר בין Tasks ל-Project
            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.Project)  // קשר ל-Project
                .WithMany(p => p.Tasks)  // לכל פרויקט יש מספר משימות
                .HasForeignKey(t => t.ProjectId)  // המפתח הזר כאן הוא ProjectId
                .OnDelete(DeleteBehavior.Restrict);  // מונע מחיקה אוטומטית של משימות אם פרויקט נמחק

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
