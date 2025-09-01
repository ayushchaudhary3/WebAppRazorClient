using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Models;
using WebAppRazorClient.Services;

namespace WebAppRazorClient.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAccountService _accountService;

        public LoginModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6)]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid) return Page();

            var req = new LoginRequest { Email = Input.Email, Password = Input.Password };

            var result = await _accountService.LoginAsync(req);
            if (!result.Success)
            {
                ErrorMessage = result.Message ?? "Invalid login attempt.";
                return Page();
            }

            if (string.IsNullOrEmpty(result.Token))
            {
                ErrorMessage = result.Message ?? "Authentication token not returned by API.";
                return Page();
            }

            // Build claims - include token + refresh info
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.Username ?? Input.Email),
                new Claim("access_token", result.Token)
            };

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", result.RefreshToken));
            }
            if (!string.IsNullOrEmpty(result.TokenType))
            {
                claims.Add(new Claim("token_type", result.TokenType));
            }
            if (result.ExpiresIn.HasValue)
            {
                claims.Add(new Claim("expires_in", result.ExpiresIn.Value.ToString()));
            }

            if (result.Roles is not null)
            {
                foreach (var r in result.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, r));
                }
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = Input.RememberMe });

            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect("/Account/Profile");
            }

            return LocalRedirect(returnUrl);
        }
    }
}