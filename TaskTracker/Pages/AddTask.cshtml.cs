using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Pages
{
    public class AddTaskModel : PageModel
    {
        private readonly TaskTrackerDbContext _context;

        [BindProperty]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public string? Description { get; set; }

        [BindProperty]
        public DateTime TaskStartDate { get; set; }

        [BindProperty]
        public DateTime TaskEndDate { get; set; }

        [BindProperty]
        public bool IsCompleted { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public AddTaskModel(TaskTrackerDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            // Default değerler
            TaskStartDate = DateTime.Now;
            TaskEndDate = DateTime.Now.AddDays(7);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            // Başlık boşsa hata
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Görev başlığı gereklidir!";
                return Page();
            }

            // Tarih kontrolü
            if (TaskStartDate >= TaskEndDate)
            {
                ErrorMessage = "Başlama tarihi, bitiş tarihinden önceki olmalıdır!";
                return Page();
            }

            // Yeni görev oluştur
            var newTask = new Models.Task
            {
                UserId = userId.Value,
                Title = Title.Trim(),
                Description = string.IsNullOrWhiteSpace(Description) ? string.Empty : Description.Trim(),
                TaskStartDate = TaskStartDate,
                TaskEndDate = TaskEndDate,
                CreatedAt = DateTime.Now,
                IsCompleted = IsCompleted
            };

            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            // Başarı mesajını göster
            SuccessMessage = "Görev başarıyla eklendi!";

            // JavaScript ile 2 saniye sonra Dashboard'a yönlendir
            return Page();
        }
    }
}
