using ClassLibrary.DTOs;
using ClassLibrary.Entities;
using Microsoft.EntityFrameworkCore;

public class EmployeeService
{
    private readonly AppDbContext _db;

    public EmployeeService(AppDbContext db)
    {
        _db = db;
    }

    // ========== READ (Get All) ==========
    public async Task<List<EmployeeDTO>> GetEmployeesAsync()
    {
        return await _db.Employees
            .Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                JobTitle = e.JobTitle,
                Salary = e.Salary,
                HireDate = e.HireDate,
                IsActive = e.IsActive
            })
            .ToListAsync();
    }

    // ========== READ (Get By ID) ==========
    public async Task<EmployeeDTO?> GetEmployeeByIdAsync(Guid id)
    {
        return await _db.Employees
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                JobTitle = e.JobTitle,
                Salary = e.Salary,
                HireDate = e.HireDate,
                IsActive = e.IsActive
            })
            .FirstOrDefaultAsync();
    }

    // ========== CREATE ==========
    public async Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO dto)
    {
        var entity = new EmployeeEntity
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            JobTitle = dto.JobTitle,
            Salary = dto.Salary,
            HireDate = dto.HireDate != default ? dto.HireDate : DateTime.UtcNow.Date,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Employees.Add(entity);
        await _db.SaveChangesAsync();

        dto.Id = entity.Id;
        return dto;
    }

    // ========== UPDATE ==========
    public async Task<EmployeeDTO?> UpdateEmployeeAsync(Guid id, EmployeeDTO dto)
    {
        var entity = await _db.Employees.FindAsync(id);
        if (entity == null)
            return null;

        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Email = dto.Email;
        entity.Phone = dto.Phone;
        entity.JobTitle = dto.JobTitle;
        entity.Salary = dto.Salary;
        entity.HireDate = dto.HireDate;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new EmployeeDTO
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            JobTitle = entity.JobTitle,
            Salary = entity.Salary,
            HireDate = entity.HireDate,
            IsActive = entity.IsActive
        };
    }

    // ========== DELETE ==========
    public async Task<bool> DeleteEmployeeAsync(Guid id)
    {
        var entity = await _db.Employees.FindAsync(id);
        if (entity == null)
            return false;

        _db.Employees.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
