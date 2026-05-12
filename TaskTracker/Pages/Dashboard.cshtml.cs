using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;

namespace TaskTracker.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly TaskTrackerDbContext _context;

        public string? UserName { get; set; }
        public int? UserId { get; set; }
        public List<TaskTracker.Models.Task> UserTasks { get; set; } = new();
        public string? SuccessMessage { get; set; }

        public DashboardModel(TaskTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserId = HttpContext.Session.GetInt32("UserId");
            UserName = HttpContext.Session.GetString("UserName");

            if (UserId == null)
            {
                return RedirectToPage("/Login");
            }

            // TempData'dan başarı mesajını al
            if (TempData.ContainsKey("SuccessMessage"))
            {
                SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            // Kullanıcının görevlerini veritabanından çek
            UserTasks = await _context.Tasks
                .Where(t => t.UserId == UserId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public async Task<IActionResult> OnPostDeleteTaskAsync(int taskId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            // Silmek istediği görevin sahibi mi kontrol et
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);

            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Görev başarıyla silindi!";
            }

            return RedirectToPage("/Dashboard");
        }
    }
}
