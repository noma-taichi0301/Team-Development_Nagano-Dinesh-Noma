using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeamDevelopment_team1.Models;
using TeamDevelopment_team1.Repositories;

namespace TeamDevelopment_team1.Pages.Todos
{
    public class CreateModel : PageModel
    {
        private readonly ITodoRepository _repo;

        public CreateModel(ITodoRepository repo)
        {
            _repo = repo;
        }

        // [BindProperty] tells ASP.NET Core to fill this object
        // from the form fields when the user submits the form.
        // Each form field name must match the property path:
        //   <input name="Input.Title" />  →  Input.Title
        [BindProperty]
        public TodoInputModel Input { get; set; } = new TodoInputModel();

        // ── OnGet ─────────────────────────────────────────────────────
        // The browser sends a GET request when the user clicks "Add Task".
        // We just render the empty form — nothing to load from DB.
        public IActionResult OnGet()
        {
            return Page();
        }

        // ── OnPost ────────────────────────────────────────────────────
        // The browser sends a POST request when the user submits the form.
        // Steps: validate → build TodoItem → save → redirect
        public IActionResult OnPost()
        {
            // Extra check: [Required] catches null but NOT whitespace-only
            // e.g. a title of "     " (5 spaces) passes [Required]
            // so we add this manual check
            if (string.IsNullOrWhiteSpace(Input.Title))
            {
                ModelState.AddModelError(
                    nameof(Input.Title),
                    "Task name cannot be blank or spaces only.");
            }

            // ModelState.IsValid is false if ANY validation attribute failed
            // ([Required], [StringLength], or our manual check above)
            if (!ModelState.IsValid)
            {
                // Return the same page — Razor will show the error messages
                return Page();
            }

            // Build a TodoItem from the form data
            TodoItem todo = new TodoItem
            {
                Title = Input.Title.Trim(),
                Detail = string.IsNullOrWhiteSpace(Input.Detail)
                               ? null
                               : Input.Detail.Trim(),
                DueDate = Input.DueDate,
                Priority = Input.Priority
            };

            try
            {
                // Save to database — synchronous, no await
                _repo.Create(todo);

                // After a successful save, redirect to the list page
                // (PRG pattern: prevents duplicate submission on F5)
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Show the error on the form instead of crashing
                ModelState.AddModelError("", "Could not save task: " + ex.Message);
                return Page();
            }
        }
    }
}
