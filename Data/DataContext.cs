using Microsoft.EntityFrameworkCore;
using schoolRegistration.Models;

namespace schoolRegistration.Data {
    public class DataContext : DbContext {
        public DataContext (DbContextOptions<DataContext> options) : base (options) { }
        public DbSet<Student> Student { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating (ModelBuilder builder) {
            
            base.OnModelCreating (builder);
            builder.Entity<Student> ().HasKey (q => q.id);
            builder.Entity<Course> ().HasKey (q => q.id);
            builder.Entity<Registration> ()
                .HasOne (s => s.Student)
                .WithMany (c => c.Registrations)
                .HasForeignKey (s => s.student_id);

            builder.Entity<Registration> ()
                .HasOne (s => s.Course)
                .WithMany (c => c.Registrations)
                .HasForeignKey (s => s.course_id);
        }

    }
}