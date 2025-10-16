using Microsoft.EntityFrameworkCore;
using EntityFrameworkDemo.Models;

namespace EntityFrameworkDemo.Data
{
    /// <summary>
    /// DbContext is the main class that coordinates Entity Framework functionality.
    /// It represents a session with the database and can be used to query and save instances of your entities.
    /// Think of it as a bridge between your C# objects and the database.
    /// </summary>
    public class SchoolContext : DbContext
    {
        /// <summary>
        /// Constructor that accepts DbContextOptions
        /// This allows us to configure the database connection and other options
        /// </summary>
        /// <param name="options">Configuration options for the database context</param>
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet represents a collection of entities that can be queried from the database.
        /// Each DbSet corresponds to a table in the database.
        /// 
        /// Students - represents the Students table
        /// Courses - represents the Courses table  
        /// Enrollments - represents the Enrollments table (junction table)
        /// </summary>
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        /// <summary>
        /// OnModelCreating is called when the model is being created.
        /// This is where we can configure relationships, constraints, and other model aspects
        /// using the Fluent API (as opposed to Data Annotations).
        /// </summary>
        /// <param name="modelBuilder">Used to configure the model</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                // Set table name explicitly (optional - EF will use class name by default)
                entity.ToTable("Students");

                // Configure the primary key (optional - EF detects Id properties automatically)
                entity.HasKey(s => s.Id);

                // Configure properties
                entity.Property(s => s.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(s => s.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(s => s.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                // Configure the one-to-many relationship with Enrollments
                entity.HasMany(s => s.Enrollments)
                    .WithOne(e => e.Student)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade); // If student is deleted, delete their enrollments too
            });

            // Configure the Course entity
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.CourseCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(c => c.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.Description)
                    .HasMaxLength(1000);

                // Configure the one-to-many relationship with Enrollments
                entity.HasMany(c => c.Enrollments)
                    .WithOne(e => e.Course)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade); // If course is deleted, delete enrollments too
            });

            // Configure the Enrollment entity (junction table)
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollments");

                entity.HasKey(e => e.Id);

                // Configure foreign key relationships
                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CourseId);

                // Configure properties
                entity.Property(e => e.Grade)
                    .HasMaxLength(2);

                entity.Property(e => e.EnrollmentDate)
                    .HasDefaultValueSql("GETDATE()"); // SQL Server function for current date/time

                // Create a composite index to prevent duplicate enrollments
                entity.HasIndex(e => new { e.StudentId, e.CourseId })
                    .IsUnique()
                    .HasDatabaseName("IX_Enrollment_Student_Course");
            });

            // Seed initial data - this runs when the database is created
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Seeds the database with initial sample data
        /// This method is called from OnModelCreating to populate the database with test data
        /// </summary>
        /// <param name="modelBuilder">Model builder used to configure seed data</param>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Students
            modelBuilder.Entity<Student>().HasData(
                new Student 
                { 
                    Id = 1, 
                    FirstName = "John", 
                    LastName = "Doe", 
                    Email = "john.doe@university.edu",
                    DateOfBirth = new DateTime(2000, 5, 15)
                },
                new Student 
                { 
                    Id = 2, 
                    FirstName = "Jane", 
                    LastName = "Smith", 
                    Email = "jane.smith@university.edu",
                    DateOfBirth = new DateTime(1999, 8, 22)
                },
                new Student 
                { 
                    Id = 3, 
                    FirstName = "Bob", 
                    LastName = "Johnson", 
                    Email = "bob.johnson@university.edu",
                    DateOfBirth = new DateTime(2001, 3, 10)
                }
            );

            // Seed Courses
            modelBuilder.Entity<Course>().HasData(
                new Course 
                { 
                    Id = 1, 
                    CourseCode = "CSCI473", 
                    Title = "C#", 
                    Description = "C#",
                    Credits = 3
                },
                new Course 
                { 
                    Id = 2, 
                    CourseCode = "CSCI470", 
                    Title = "Java Development", 
                    Description = "Java",
                    Credits = 3
                },
                new Course 
                { 
                    Id = 3, 
                    CourseCode = "CSCI467", 
                    Title = "Software Engineering", 
                    Description = "Software development methodologies and practices",
                    Credits = 3
                }
            );

            // Seed Enrollments
            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { Id = 1, StudentId = 1, CourseId = 1, Grade = "A", EnrollmentDate = DateTime.Now.AddDays(-30) },
                new Enrollment { Id = 2, StudentId = 1, CourseId = 2, Grade = "B+", EnrollmentDate = DateTime.Now.AddDays(-25) },
                new Enrollment { Id = 3, StudentId = 2, CourseId = 1, Grade = "A-", EnrollmentDate = DateTime.Now.AddDays(-20) },
                new Enrollment { Id = 4, StudentId = 2, CourseId = 3, Grade = null, EnrollmentDate = DateTime.Now.AddDays(-15) },
                new Enrollment { Id = 5, StudentId = 3, CourseId = 2, Grade = "B", EnrollmentDate = DateTime.Now.AddDays(-10) }
            );
        }
    }
}
