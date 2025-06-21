using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using WebApp.Services;

namespace WebApp.Pages.Account
{
    public class RegisterModel(UserManager<IdentityUser> userManager, IEmailService emailService) : PageModel
    {
        [BindProperty]
        public RegisterViewModel ViewModel { get; set; } = new RegisterViewModel();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create the user  
            var user = new IdentityUser
            {
                Email = ViewModel.Email,
                UserName = ViewModel.Email
            };

            var result = await userManager.CreateAsync(user, ViewModel.Password);

            if (result.Succeeded)
            {
                var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = user.Id, token = confirmationToken },
                    protocol: Request.Scheme);

                await emailService.SendAsync("rabbifazla4@gmail.com",
                    user.Email,
                    "Please confirm your email",
                    $"Please click on this link to confirm your email address: {confirmationLink}");

                return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return Page();
            }
        }

        public class RegisterViewModel
        {
            [Required]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(dataType: DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }
    }
}