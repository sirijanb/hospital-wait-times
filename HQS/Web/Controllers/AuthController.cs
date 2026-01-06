using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HQS.Infrastructure.Identity;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
       
        if (user == null)
            // return BadRequest("Invalid credentials");
            // return RedirectToAction("Login", new { error = "invalid" });
            return LocalRedirect("/login?error=invalidcredentials");

        // var roles = await _userManager.GetRolesAsync(user);
        var isAdmin = await _userManager.IsInRoleAsync(user, "MasterAdmin");
        

        if (!user.IsApproved && !isAdmin)
            // return Unauthorized("Account not approved. Contact Admin.");
            // return RedirectToAction("Login", new { error = "unapproved" });
            return LocalRedirect("/login?error=unapproved");

        var result = await _signInManager.PasswordSignInAsync(
            user,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );
        
        if (!result.Succeeded)
            // return BadRequest("Invalid credentials");
            // return RedirectToAction("Login", new { error = "invalid" });
            return LocalRedirect("/login?error=invalidcredentials");
        
        if (isAdmin)
        {
            return LocalRedirect("/admin");
        }
        else
        {
            return LocalRedirect("/update-info"); // Or wherever the default page is
        }
        // return Ok(
        //     new { role = roles.FirstOrDefault() ?? "HospitalRep" }
        // );
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpGet("logout")]
    public async Task<IActionResult> LogoutGet()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/login");
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
