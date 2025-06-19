using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages.Account
{
    public class LoginModel(SignInManager<IdentityUser> signInManager) : PageModel
    {
        private readonly SignInManager<IdentityUser> signInManager = signInManager;

        [BindProperty]
        public CredentialViewModel credentialViewModel { get; set; }

        public void OnGet()
        {
        }

        //public IActionResult OnPost()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    // Add your login logic here.  
        //    return RedirectToPage("/Index");
        //}


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await signInManager.PasswordSignInAsync(
                this.credentialViewModel.Email,
                this.credentialViewModel.Password,
                this.credentialViewModel.RememberMe
                , false);

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
