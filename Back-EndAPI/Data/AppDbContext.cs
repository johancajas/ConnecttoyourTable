using ClassLibrary.DTOs;
using ClassLibrary.Entities;
using Microsoft.EntityFrameworkCore;

//
// DB CONTEXT ROLE
// ----------------
// AppDbContext represents the DATABASE.
// Each DbSet<T> represents a TABLE.
//
// EF Core uses this class to:
// - Generate SQL
// - Track changes
// - Execute queries safely
//
// This class should contain NO business logic.
//

public class AppDbContext : DbContext
{
    // DbSet = a database table
    public DbSet<CharacterEntity> Characters => Set<CharacterEntity>();
    public DbSet<EmployeeEntity> Employees => Set<EmployeeEntity>();

    // Constructor receives configuration options
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
