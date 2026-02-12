namespace ClassLibrary.DTOs
{
    public class EmployeeDTO
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? JobTitle { get; set; }

        public decimal? Salary { get; set; }

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; }
    }
}
