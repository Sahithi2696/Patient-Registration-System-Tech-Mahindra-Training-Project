using System.ComponentModel.DataAnnotations;

namespace PatientReg.Api.Models;

public class Patient
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = default!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = default!;

    [Required]                // yyyy-MM-dd on the wire
    public DateOnly Dob { get; set; }

    [Required, MaxLength(20)]
    public string Gender { get; set; } = "Other"; // Male/Female/Other

    [Required, MaxLength(30)]
    public string Phone { get; set; } = default!;

    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }

    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }

    public string? InsuranceProvider { get; set; }
    public string? InsuranceNumber { get; set; }

    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
