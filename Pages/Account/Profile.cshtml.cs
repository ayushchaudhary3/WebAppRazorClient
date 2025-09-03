using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace WebAppRazorClient.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        public string Username { get; private set; } = string.Empty;
        public string? Email { get; private set; }
        public string[]? Roles { get; private set; }
        public string? Token { get; private set; }

        public void OnGet()
        {
            Username = User.Identity?.Name ?? "Unknown";

        }
    }
}