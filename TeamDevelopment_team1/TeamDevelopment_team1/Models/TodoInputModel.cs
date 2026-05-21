using System.ComponentModel.DataAnnotations;

namespace TeamDevelopment_team1.Models
{
    public class TodoInputModel
    {
        // [Required]     = このフィールドは空にできません
        // [StringLength] = 最大100文字まで入力可能
        // [Display]      = 入力ボックスの横に表示されるラベルテキスト
        [Required(ErrorMessage = "タスク名は必須です。")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "タスク名は1文字から150文字までで入力してください。")]
        [Display(Name = "タスク名")]
        public string Title { get; set; } = "";

        [StringLength(1000, ErrorMessage = "詳細記述は1000文字以内としてください。")]
        [Display(Name = "詳細")]
        public string? Detail { get; set; }

        [Display(Name = "期限日")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "優先度")]
        public Priority Priority { get; set; } = Priority.Mid;

        // ── NEW: Assignee ─────────────────────────────────────
        // Nullable int — user may leave this blank (no assignee)
        [Display(Name = "Assignee")]
        public int? AssigneeId { get; set; }
    }
}
