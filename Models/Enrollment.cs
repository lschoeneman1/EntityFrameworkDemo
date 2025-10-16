using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkDemo.Models
{
    /// <summary>
    /// Enrollment entity represents a many-to-many relationship between Students and Courses.
    /// This is a junction table that stores additional information about the relationship.
    /// In Entity Framework, this is called a "join entity" or "associative entity".
    /// </summary>
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to Student - this creates the relationship
        // The [ForeignKey] attribute explicitly defines the foreign key relationship
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        // Foreign key to Course
        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }

        // Grade for the enrollment (can be null if not yet graded)
        [MaxLength(2)]
        public string? Grade { get; set; } // A, B, C, D, F, or null

        // Enrollment date
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        // Navigation properties - these allow you to navigate from Enrollment to related entities
        // These are how Entity Framework implements relationships between tables
        public virtual Student Student { get; set; } = null!; // The ! tells the compiler this won't be null
        public virtual Course Course { get; set; } = null!;

        /// <summary>
        /// Override ToString for better display in console output
        /// </summary>
        public override string ToString()
        {
            var gradeText = Grade ?? "Not Graded";
            return $"{Student?.FullName} enrolled in {Course?.Title} (Grade: {gradeText})";
        }
    }
}
