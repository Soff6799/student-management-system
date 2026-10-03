using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.WebApi.DTOs.Organization;

public class OrganizationUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string ShortName { get; set; } = string.Empty;

    [Required]
    [MaxLength(12)]
    public string INN { get; set; } = string.Empty;

    [Required]
    [MaxLength(9)]
    public string KPP { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string OGRN { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string LegalAddress { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ActualAddress { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(255)]
    public string? ContactPersonFullName { get; set; }

    [MaxLength(255)]
    public string? ContactPersonPosition { get; set; }

    [MaxLength(2000)]
    public string? Note { get; set; }
}