using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages.Account
{
    public class LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : PageModel
    {

        [BindProperty]
        public CredentialViewModel credential { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.FindByEmailAsync(credential.Email);
            if (user == null)
            {
                ModelState.AddModelError("Login", "User not found.");
                return Page();
            }

            var userFailedAttempts = await userManager.GetAccessFailedCountAsync(user);

            var result = await signInManager.PasswordSignInAsync(
                credential.Email,
                credential.Password,
                credential.RememberMe,
                false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to Login");
                }

                return Page();
            }
        }

        public class CredentialViewModel
        {
            [Required]
            public string Email { get; set; } = string.Empty;
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
            [Display(Name = "Remember Me")]
            public bool RememberMe { get; set; }
        }
    }
}
