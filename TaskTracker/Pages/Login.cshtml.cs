using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using System.Security.Cryptography;
using System.Text;

namespace TaskTracker.Pages
{
    public class LoginModel : PageModel
    {
        private readonly TaskTrackerDbContext _context;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public LoginModel(TaskTrackerDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            if (TempData.ContainsKey("SuccessMessage"))
            {
                SuccessMessage = TempData["SuccessMessage"].ToString();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null || !VerifyPassword(Password, user.Password))
            {
                ErrorMessage = "Email veya şifre yanlış!";
                return Page();
            }

            // Başarılı giriş - session'a user bilgisini kaydet
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FirstName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            await HttpContext.Session.CommitAsync();

            return RedirectToPage("/Dashboard");
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedInput = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                var hashString = Convert.ToBase64String(hashedInput);
                return hashString == storedHash;
            }
        }
    }
}
