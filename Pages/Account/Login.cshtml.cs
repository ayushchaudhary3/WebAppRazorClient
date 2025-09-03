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

        [BindProperty] // Bind form values to this property
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

        // Handle form submission
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

            //Create list of claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.Username ?? Input.Email),
                new Claim("access_token", result.Token)
            };

            if (result.Roles is not null)
            {
                foreach (var r in result.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, r));
                }
            }

            // Create claims identity and principal represents logged-in user
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); 
            var principal = new ClaimsPrincipal(identity);

            //This makes the authentication cookie so the user stays logged in
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = Input.RememberMe });

            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect("/");
            }

            return LocalRedirect(returnUrl); // Redirect to return URL if valid
        }
    }
}