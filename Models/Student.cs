using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkDemo.Models
{
    /// <summary>
    /// Student entity represents a student in our database.
    /// This class maps to a 'Students' table in the database.
    /// Entity Framework uses these classes to create and manage database tables.
    /// </summary>
    public class Student
    {
        // Primary key - Entity Framework automatically recognizes properties named 'Id' or 'StudentId' as primary keys
        // The [Key] attribute makes this explicit
        [Key]
        public int Id { get; set; }   

        // Required field - Entity Framework will create this as NOT NULL in the database
        [Required]
        [MaxLength(100)] // Maximum length constraint
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        // Email field with validation
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        // Date of birth
        public DateTime DateOfBirth { get; set; }

        // Navigation property - represents the relationship between Student and Enrollment
        // This is a one-to-many relationship: one student can have many enrollments
        // The 'virtual' keyword enables lazy loading (loading related data when needed)
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        /// <summary>
        /// Computed property that returns the full name
        /// Note: This won't be stored in the database since it's just a property getter
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Computed property that calculates age
        /// </summary>
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// Override ToString for better display in console output
        /// </summary>
        public override string ToString()
        {
            return $"{FullName} (Age: {Age}, Email: {Email})";
        }
    }
}
