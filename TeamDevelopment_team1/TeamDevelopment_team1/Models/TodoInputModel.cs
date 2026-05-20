using System.ComponentModel.DataAnnotations;

namespace TeamDevelopment_team1.Models
{
    public class TodoInputModel
    {
        // [Required]     = このフィールドは空にできません
        // [StringLength] = 最大100文字まで入力可能
        // [Display]      = 入力ボックスの横に表示されるラベルテキスト
        [Required(ErrorMessage = "Task name is required.")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Task name must be 1 to 150 characters.")]
        [Display(Name = "Task Name")]
        public string Title { get; set; } = "";

        [StringLength(1000, ErrorMessage = "Details cannot exceed 1000 characters.")]
        [Display(Name = "Details")]
        public string? Detail { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Priority")]
        public Priority Priority { get; set; } = Priority.Mid;
    }
}
