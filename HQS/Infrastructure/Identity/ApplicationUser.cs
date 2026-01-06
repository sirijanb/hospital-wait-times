using Microsoft.AspNetCore.Identity;

namespace HQS.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IsApproved { get; set; } = false;
    public string? HospitalId { get; set; }
}
