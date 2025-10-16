# Entity Framework Core Demo

This console application demonstrates the basics of Entity Framework Core for C# students. It shows how to create a database, perform CRUD operations, and work with relationships between entities.

## What You'll Learn

- **Code First Approach**: Creating database tables from C# classes
- **Entity Models**: Defining entities with properties and relationships
- **DbContext**: The central class that coordinates EF functionality
- **CRUD Operations**: Create, Read, Update, and Delete database records
- **Relationships**: One-to-many and many-to-many relationships
- **LINQ Queries**: Using LINQ with Entity Framework for data retrieval
- **Data Annotations**: Using attributes to configure entity properties
- **Fluent API**: Alternative way to configure entity relationships

## Prerequisites

- .NET 8.0 SDK
- SQL Server LocalDB (comes with Visual Studio)
- Visual Studio 2022 or VS Code

## Getting Started

1. **Clone or download** this project
2. **Open a terminal/command prompt** in the project directory
3. **Restore packages**:
   ```bash
   dotnet restore
   ```
4. **Run the application**:
   ```bash
   dotnet run
   ```

## Project Structure

```
EntityFrameworkDemo/
├── Models/
│   ├── Student.cs          # Student entity
│   ├── Course.cs           # Course entity
│   └── Enrollment.cs       # Enrollment entity (junction table)
├── Data/
│   └── SchoolContext.cs    # DbContext class
├── Program.cs              # Main application with CRUD demos
├── EntityFrameworkDemo.csproj
└── README.md
```

## Key Concepts Demonstrated

### 1. Entity Models
- **Student**: Represents a student with basic information
- **Course**: Represents a course with code, title, and credits
- **Enrollment**: Junction table linking students to courses with grades

### 2. Relationships
- **One-to-Many**: One student can have many enrollments
- **One-to-Many**: One course can have many enrollments
- **Many-to-Many**: Students can enroll in multiple courses, courses can have multiple students

### 3. CRUD Operations
- **Create**: Adding new students, courses, and enrollments
- **Read**: Querying data with LINQ (filtering, ordering, grouping)
- **Update**: Modifying existing records
- **Delete**: Removing records (with cascade delete)

### 4. Advanced Features
- **Eager Loading**: Using `Include()` to load related data
- **Aggregation**: Count, average, and other statistical operations
- **Complex Queries**: Joins, filtering, and grouping

## Database Connection

The application uses SQL Server LocalDB with this connection string:
```
Server=(localdb)\mssqllocaldb;Database=SchoolDb;Trusted_Connection=true;
```

The database file is created automatically in your user profile directory.

## Running the Demo

When you run the application, it will:

1. **Create the database** (if it doesn't exist)
2. **Seed initial data** (students, courses, enrollments)
3. **Demonstrate CRUD operations**:
   - Display existing data
   - Add new records
   - Update existing records
   - Delete records
   - Show advanced queries

## Key Entity Framework Concepts

### DbContext
- Represents a session with the database
- Tracks changes to entities
- Manages database connections
- Provides LINQ query capabilities

### DbSet
- Represents a collection of entities
- Maps to a database table
- Provides methods for CRUD operations

### Change Tracking
- EF automatically tracks changes to loaded entities
- Call `SaveChanges()` to persist changes to database
- Supports optimistic concurrency control

### LINQ Integration
- Use LINQ queries with EF
- Queries are translated to SQL
- Supports filtering, ordering, grouping, joins

## Common Patterns

### Querying Data
```csharp
// Get all students
var students = await context.Students.ToListAsync();

// Filter students
var johns = await context.Students
    .Where(s => s.FirstName == "John")
    .ToListAsync();

// Include related data
var studentsWithEnrollments = await context.Students
    .Include(s => s.Enrollments)
    .ToListAsync();
```

### Adding Data
```csharp
var student = new Student { FirstName = "Alice", LastName = "Smith" };
context.Students.Add(student);
await context.SaveChangesAsync();
```

### Updating Data
```csharp
var student = await context.Students.FindAsync(1);
student.FirstName = "Alice Updated";
await context.SaveChangesAsync();
```

### Deleting Data
```csharp
var student = await context.Students.FindAsync(1);
context.Students.Remove(student);
await context.SaveChangesAsync();
```

## Troubleshooting

### SQL Server LocalDB Issues
If you get connection errors:
1. Make sure SQL Server LocalDB is installed
2. Try running: `sqllocaldb start`
3. Check if LocalDB is running: `sqllocaldb info`

### Database Already Exists
If you want to start fresh:
1. Delete the database file from your LocalDB directory
2. Or change the database name in the connection string

## Next Steps

After running this demo, try:
1. Adding new entity properties
2. Creating new relationships
3. Implementing your own CRUD operations
4. Adding data validation
5. Using migrations for database versioning

## Additional Resources

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [LINQ Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
