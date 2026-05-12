using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;
using System.Security.Cryptography;
using System.Text;

namespace TaskTracker.Pages
{
    public class KayitModel : PageModel
    {
        private readonly TaskTrackerDbContext _context;

        [BindProperty]
        public string FirstName { get; set; } = string.Empty;

        [BindProperty]
        public string LastName { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string PasswordConfirm { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public KayitModel(TaskTrackerDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Şifre kontrolü
            if (Password != PasswordConfirm)
            {
                ErrorMessage = "Şifreler eşleşmiyor!";
                return Page();
            }

            // Email benzersizliği kontrolü
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (existingUser != null)
            {
                ErrorMessage = "Bu email zaten kullanılıyor!";
                return Page();
            }

            // Yeni kullanıcı oluştur
            var newUser = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = HashPassword(Password),
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // TempData ile başarı mesajı gönder
            TempData["SuccessMessage"] = "Kayıt başarılı! Lütfen giriş yapın.";
            return RedirectToPage("/Login");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedInput = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedInput);
            }
        }
    }
}
