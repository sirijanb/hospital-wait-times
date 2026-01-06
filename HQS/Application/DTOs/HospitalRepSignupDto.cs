namespace HQS.Application.DTOs;

public class HospitalRepSignupDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid HospitalId { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
}
