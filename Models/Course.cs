using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkDemo.Models
{
    /// <summary>
    /// Course entity represents a course in our database.
    /// This class maps to a 'Courses' table in the database.
    /// </summary>
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string CourseCode { get; set; } = string.Empty; // e.g., "CSCI473"

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty; // e.g., "Database Systems"

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Credits for the course
        [Range(1, 6)] // Courses typically have 1-6 credits
        public int Credits { get; set; }

        // Navigation property - represents the relationship between Course and Enrollment
        // This is a one-to-many relationship: one course can have many enrollments
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        /// <summary>
        /// Override ToString for better display in console output
        /// </summary>
        public override string ToString()
        {
            return $"{CourseCode}: {Title} ({Credits} credits)";
        }
    }
}
