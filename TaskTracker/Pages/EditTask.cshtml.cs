using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;

namespace TaskTracker.Pages
{
    public class EditTaskModel : PageModel
    {
        private readonly TaskTrackerDbContext _context;

        [BindProperty]
        public int TaskId { get; set; }

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

        public EditTaskModel(TaskTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int taskId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            // Görevin sahibi mi kontrol et
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);
            
            if (task == null)
            {
                return RedirectToPage("/Dashboard");
            }

            TaskId = task.TaskId;
            Title = task.Title;
            Description = task.Description;
            TaskStartDate = task.TaskStartDate;
            TaskEndDate = task.TaskEndDate;
            IsCompleted = task.IsCompleted;

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

            // Görevin sahibi mi kontrol et
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == TaskId && t.UserId == userId);
            
            if (task == null)
            {
                return RedirectToPage("/Dashboard");
            }

            // Güncelle
            task.Title = Title.Trim();
            task.Description = string.IsNullOrWhiteSpace(Description) ? string.Empty : Description.Trim();
            task.TaskStartDate = TaskStartDate;
            task.TaskEndDate = TaskEndDate;
            task.IsCompleted = Request.Form["IsCompleted"] == "true";

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            SuccessMessage = "Görev başarıyla güncellendi!";
            
            // JavaScript ile 2 saniye sonra Dashboard'a yönlendir
            return Page();
        }
    }
}
