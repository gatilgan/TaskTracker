namespace TaskTracker.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TaskStartDate { get; set; }
        public DateTime TaskEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCompleted { get; set; } = false;

        public User? User { get; set; }
        
        // Görev süresini hesapla
        public int DurationHours => (TaskEndDate - TaskStartDate).Hours;
        public int DurationMinutes => (TaskEndDate - TaskStartDate).Minutes;

        public string TaskDuration => DurationHours == 0 ? $"{DurationMinutes} dakika" : (DurationMinutes == 0 ? $"{DurationHours} saat" : $"{DurationHours} saat {DurationMinutes} dakika");

        // Görev durumunu döndür
        public string Status => IsCompleted ? "Tamamlandı" : "Devam Ediyor";
    }
}
