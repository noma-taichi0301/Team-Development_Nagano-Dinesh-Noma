namespace TeamDevelopment_team1.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Detail { get; set; }
        public string PriorityLabel { get; set; }
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Mid;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        // UI-only properties computed inside OnGet
        public bool IsOverdue { get; set; }
        public bool IsDueToday { get; set; }
        public string PriorityBadgeClass { get; set; }
    }
}
