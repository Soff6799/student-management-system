namespace StudentAccounting.WebApi.DTOs.Organization;

public class OrganizationDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string INN { get; set; } = string.Empty;
    public string KPP { get; set; } = string.Empty;
    public string OGRN { get; set; } = string.Empty;
    public string LegalAddress { get; set; } = string.Empty;
    public string? ActualAddress { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ContactPersonFullName { get; set; }
    public string? ContactPersonPosition { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}