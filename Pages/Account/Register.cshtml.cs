using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppRazorClient.Models;
using WebAppRazorClient.Services;

namespace WebAppRazorClient.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IAccountService _accountService;

        public RegisterModel(IAccountService accountService) 
        {
            _accountService = accountService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6)]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var req = new RegisterRequest { Email = Input.Email, Password = Input.Password };

            var result = await _accountService.RegisterAsync(req);
            if (!result.Success)
            {
                StatusMessage = result.Message ?? "Registration failed.";
                return Page();
            }

            // Registration succeeded — redirect to login
            return RedirectToPage("Login");
        }
    }
}