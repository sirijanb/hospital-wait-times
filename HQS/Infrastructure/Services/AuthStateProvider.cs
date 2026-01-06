using HQS.Infrastructure.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace HQS.Infrastructure.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthStateProvider(IHttpContextAccessor httpContextAccessor,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User
                       ?? new ClaimsPrincipal(new ClaimsIdentity());

            return Task.FromResult(new AuthenticationState(user));
        }

        /// <summary>
        /// Call after login
        /// </summary>
        public void NotifyUserAuthentication(ClaimsPrincipal user)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        /// <summary>
        /// Call after logout (updates Blazor UI only)
        /// </summary>
        public void NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        /// <summary>
        /// Fully logs out user including Identity cookie
        /// </summary>
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();  // clears Identity cookie
            NotifyUserLogout();                  // updates Blazor UI
        }
    }
}