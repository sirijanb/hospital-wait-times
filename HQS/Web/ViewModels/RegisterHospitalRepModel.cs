using System.ComponentModel.DataAnnotations;

namespace HQS.Web.ViewModels;

public class RegisterHospitalRepModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string HospitalId { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[0-9]{10}$", 
    ErrorMessage = "Phone must be exactly 10 digits with no spaces or symbols (e.g., 5142888201)")]
    public string ContactNumber { get; set; } = string.Empty;
}
