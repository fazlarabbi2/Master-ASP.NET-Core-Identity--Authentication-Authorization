using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
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
                // Generate a URL for the email confirmation page with encoded token
                string? confirmationLink = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = user.Id, token = confirmationToken },
                    protocol: Request.Scheme);

                if (confirmationLink == null)
                {
                    ModelState.AddModelError("Register", "Failed to generate confirmation link.");
                    return Page();
                }

                // Create a more professional HTML email
                string userName = ViewModel.Email.Split('@')[0]; // Use part before @ as name
                string emailHtml = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            max-width: 600px;
                            margin: 0 auto;
                            text-align: start;
                        }}
                        .container {{
                            padding: 20px;
                            border: 1px solid #ddd;
                            border-radius: 5px;
                        }}
                        .header {{
                            background-color: #4a6fdc;
                            color: white;
                            padding: 10px 20px;
                            border-radius: 5px 5px 0 0;
                            margin-bottom: 20px;
                        }}
                        .button {{
                            display: inline-block;
                            background-color: #4a6fdc;
                            color: white;
                            padding: 12px 24px;
                            text-decoration: none;
                            border-radius: 4px;
                            margin: 20px 0;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 12px;
                            color: #777;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Welcome to Our Application!</h2>
                        </div>
                        <p>Hello {userName},</p>
                        <p>Thank you for registering with our application. To complete your registration and verify your email address, please click the button below:</p>
                        
                        <a href='{confirmationLink}' class='button'>Confirm Email Address</a>
                        
                        <p>If the button doesn't work, you can copy and paste the following link into your browser:</p>
                        <p>{confirmationLink}</p>
                        
                        <p>This link will expire in 24 hours for security reasons.</p>
                        
                        <p>If you did not create an account with us, please disregard this email.</p>
                        
                        <div class='footer'>
                            <p>&copy; {DateTime.Now.Year} Your Application Name. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

                await emailService.SendAsync(
                    "rabbifazla4@gmail.com",
                    user.Email,
                    "Please confirm your email address",
                    emailHtml);
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