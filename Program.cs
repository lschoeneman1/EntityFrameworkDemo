using EntityFrameworkDemo.Data;
using EntityFrameworkDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkDemo;

/// <summary>
///     Main program demonstrating Entity Framework Core basics
///     This console application shows how to:
///     1. Create a database using Code First approach
///     2. Perform CRUD operations (Create, Read, Update, Delete)
///     3. Work with relationships between entities
///     4. Use LINQ queries with Entity Framework
/// </summary>
internal class Program
{
    /// <summary>
    ///     Main entry point of the application
    /// </summary>
    private static async Task Main(string[] args)
    {
        Console.WriteLine("=== Entity Framework Core Demo ===");
        Console.WriteLine("This demo shows basic EF Core operations\n");

        // Configure database options
        // In a real application, you'd typically get connection string from configuration
        var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();

        // Using SQL Server LocalDB - a lightweight version of SQL Server
        // This creates a database file in the user's profile directory
        // You can also use SQLite, PostgreSQL, MySQL, etc.
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SchoolDb;Trusted_Connection=true;");

        // Create the context with the configured options
        var options = optionsBuilder.Options;

        try
        {
            // Demonstrate database creation and operations
            await DemonstrateDatabaseOperations(options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Make sure SQL Server LocalDB is installed and running.");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    ///     Demonstrates various Entity Framework operations
    /// </summary>
    /// <param name="options">Database context options</param>
    private static async Task DemonstrateDatabaseOperations(DbContextOptions<SchoolContext> options)
    {
        // CREATE - Database Creation
        Console.WriteLine("1. Creating database...");
        await CreateDatabase(options);

        // READ - Querying data
        Console.WriteLine("\n2. Reading data from database...");
        await ReadData(options);

        // CREATE - Adding new records
        Console.WriteLine("\n3. Adding new records...");
        await CreateData(options);

        // UPDATE - Modifying existing records
        Console.WriteLine("\n4. Updating existing records...");
        await UpdateData(options);

        // DELETE - Removing records
        Console.WriteLine("\n5. Deleting records...");
        await DeleteData(options);

        // Advanced queries
        Console.WriteLine("\n6. Advanced queries and relationships...");
        await AdvancedQueries(options);
    }

    /// <summary>
    ///     Creates the database and tables using Entity Framework migrations
    /// </summary>
    /// <param name="options">Database context options</param>
    private static async Task CreateDatabase(DbContextOptions<SchoolContext> options)
    {
        await using var context = new SchoolContext(options);

        // EnsureCreated() creates the database if it doesn't exist
        // This is useful for development, but in production you'd typically use migrations
        var created = await context.Database.EnsureCreatedAsync();

        if (created)
        {
            Console.WriteLine("✓ Database created successfully!");
        }
        else
        {
            Console.WriteLine("✓ Database already exists.");
        }

        // Alternative approach using migrations (commented out for this demo):
        // await context.Database.MigrateAsync();
    }

    /// <summary>
    ///     Demonstrates reading data from the database
    /// </summary>
    /// <param name="options">Database context options</param>
    private static async Task ReadData(DbContextOptions<SchoolContext> options)
    {
        await using var context = new SchoolContext(options);

        // Simple query - get all students
        Console.WriteLine("All Students:");
        var students = await context.Students.ToListAsync();
        foreach (var student in students)
        {
            Console.WriteLine($"  - {student}");
        }

        // Query with filtering
        Console.WriteLine("\nStudents with 'J' in their first name:");
        var studentsWithJ = await context.Students
                                         .Where(s => s.FirstName.Contains("J"))
                                         .ToListAsync();
        foreach (var student in studentsWithJ)
        {
            Console.WriteLine($"  - {student}");
        }

        // Query with ordering
        Console.WriteLine("\nCourses ordered by title:");
        var courses = await context.Courses
                                   .OrderBy(c => c.Title)
                                   .ToListAsync();
        foreach (var course in courses)
        {
            Console.WriteLine($"  - {course}");
        }
    }

    /// <summary>
    ///     Demonstrates creating new records in the database
    /// </summary>
    /// <param name="options">Database context options</param>
    private static async Task CreateData(DbContextOptions<SchoolContext> options)
    {
        await using var context = new SchoolContext(options);

        // Create a new student
        var newStudent = new Student
        {
            FirstName = "Alice",
            LastName = "Wonderland",
            Email = "alice.wonderland@university.edu",
            DateOfBirth = new DateTime(2002, 7, 4)
        };

        // Add the student to the context
        // Note: This doesn't save to database yet - just adds to EF's tracking
        context.Students.Add(newStudent);

        // Create a new course
        var newCourse = new Course
        {
            CourseCode = "CSCI340",
            Title = "Data Structures and Algorithms",
            Description = "Data Structures",
            Credits = 4
        };

        context.Courses.Add(newCourse);

        // Save changes to the database
        // This executes INSERT statements for all pending changes
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Added new student: {newStudent.FullName}");
        Console.WriteLine($"✓ Added new course: {newCourse.Title}");

        // Create an enrollment (relationship between student and course)
        var enrollment = new Enrollment
        {
            StudentId = newStudent.Id,
            CourseId = newCourse.Id,
            Grade = null, // Not graded yet
            EnrollmentDate = DateTime.Now
        };

        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Enrolled {newStudent.FullName} in {newCourse.Title}");
    }

    /// <summary>
    ///     Demonstrates updating existing records in the database
    /// </summary>
    /// <param name="options">Database context options</param>
    private static async Task UpdateData(DbContextOptions<SchoolContext> options)
    {
        await using var context = new SchoolContext(options);

        // Find a student to update
        var student = await context.Students
                                   .FirstOrDefaultAsync(s => s.FirstName == "John");

        if (student != null)
        {
            Console.WriteLine($"Updating student: {student.FullName}");

            // Modify the student's email
            student.Email = "john.doe.updated@university.edu";

            // EF automatically tracks changes to entities that were loaded from the database
            // No need to call Add() or Update() - EF knows this entity was modified

            // Save changes to the database
            await context.SaveChangesAsync();

            Console.WriteLine($"✓ Updated email for {student.FullName}");
        }

        // Update multiple records
        Console.WriteLine("Updating course credits...");
        var courses = await context.Courses
                                   .Where(c => c.Credits == 3)
                                   .ToListAsync();

        foreach (var course in courses)
        {
            course.Credits = 4; // Change all 3-credit courses to 4 credits
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Updated {courses.Count} courses to 4 credits");
    }

    /// <summary>
    ///     Demonstrates deleting records from the database
    /// </summary>
    /// <param name="options">Database context options</param>
    private static async Task DeleteData(DbContextOptions<SchoolContext> options)
    {
        using var context = new SchoolContext(options);

        // Find a student to delete
        var studentToDelete = await context.Students
                                           .FirstOrDefaultAsync(s => s.FirstName == "Bob");

        if (studentToDelete != null)
        {
            Console.WriteLine($"Deleting student: {studentToDelete.FullName}");

            // Remove the student from the context
            // Note: Due to cascade delete configured in the model,
            // this will also delete all enrollments for this student
            context.Students.Remove(studentToDelete);

            // Save changes to the database
            await context.SaveChangesAsync();

            Console.WriteLine($"✓ Deleted {studentToDelete.FullName} and their enrollments");
        }

        // Delete records using a query
        Console.WriteLine("Deleting enrollments without grades...");
        var enrollmentsToDelete = await context.Enrollments
                                               .Where(e => e.Grade == null)
                                               .ToListAsync();

        context.Enrollments.RemoveRange(enrollmentsToDelete);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Deleted {enrollmentsToDelete.Count} ungraded enrollments");
    }

    /// <summary>
    ///     Demonstrates advanced queries and working with relationships
    /// </summary>
    /// <param name="options">Database context options</param>
    private static async Task AdvancedQueries(DbContextOptions<SchoolContext> options)
    {
        using var context = new SchoolContext(options);

        // Include related data (Eager Loading)
        Console.WriteLine("Students with their enrollments:");
        var studentsWithEnrollments = await context.Students
                                                   .Include(s => s.Enrollments) // Load related enrollments
                                                   .ThenInclude(e =>
                                                       e.Course) // Load related course for each enrollment
                                                   .ToListAsync();

        foreach (var student in studentsWithEnrollments)
        {
            Console.WriteLine($"\n{student.FullName}:");
            foreach (var enrollment in student.Enrollments)
            {
                var grade = enrollment.Grade ?? "Not Graded";
                Console.WriteLine($"  - {enrollment.Course.Title} (Grade: {grade})");
            }
        }

        // Complex query with joins
        Console.WriteLine("\nStudents enrolled in Database Systems:");
        var databaseStudents = await context.Students
                                            .Where(s => s.Enrollments.Any(e => e.Course.Title == "Database Systems"))
                                            .ToListAsync();

        foreach (var student in databaseStudents)
        {
            Console.WriteLine($"  - {student.FullName}");
        }

        // Aggregation queries
        Console.WriteLine("\nStatistics:");
        var totalStudents = await context.Students.CountAsync();
        var totalCourses = await context.Courses.CountAsync();
        var totalEnrollments = await context.Enrollments.CountAsync();
        var averageCredits = await context.Courses.AverageAsync(c => c.Credits);

        Console.WriteLine($"  - Total Students: {totalStudents}");
        Console.WriteLine($"  - Total Courses: {totalCourses}");
        Console.WriteLine($"  - Total Enrollments: {totalEnrollments}");
        Console.WriteLine($"  - Average Course Credits: {averageCredits:F1}");

        // Group by queries
        Console.WriteLine("\nCourses by credit count:");
        var coursesByCredits = await context.Courses
                                            .GroupBy(c => c.Credits)
                                            .Select(g => new { Credits = g.Key, Count = g.Count() })
                                            .OrderBy(g => g.Credits)
                                            .ToListAsync();

        foreach (var group in coursesByCredits)
        {
            Console.WriteLine($"  - {group.Credits} credit(s): {group.Count} course(s)");
        }
    }
}