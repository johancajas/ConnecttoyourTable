using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entities
{
    [Table("employee")]
    public class EmployeeEntity
    {
        [Key]
        [Column("employee_id")]
        public Guid Id { get; set; }

        [Column("first_name")]
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Column("email")]
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Column("phone")]
        [MaxLength(25)]
        public string? Phone { get; set; }

        [Column("job_title")]
        [MaxLength(150)]
        public string? JobTitle { get; set; }

        [Column("salary")]
        public decimal? Salary { get; set; }

        [Column("hire_date")]
        public DateTime HireDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
