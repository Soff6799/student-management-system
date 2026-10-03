using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.WebApi.DTOs.Employee;

public class EmployeeCreateDto
{
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Patronymic { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(255)]
    public string? Post { get; set; }

    public Guid? OrganizationId { get; set; }
}