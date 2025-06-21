using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account
{
    public class ConfirmEmailModel(UserManager<IdentityUser> userManager) : PageModel
    {
        [BindProperty]
        public string Message { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token); // Await the Task<IdentityResult>
    
                if (result.Succeeded) // Access the Succeeded property of IdentityResult
                {
                    Message = "Email is successfully confirmed, you can try to login";
                    return Page();
                }
            }

            Message = "Failed to validate email";
            return Page();
        }
    }
}
