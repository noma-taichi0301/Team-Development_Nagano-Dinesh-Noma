using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeamDevelopment_team1.Models;
using TeamDevelopment_team1.Repositories;

// ================================================================
// Edit.cshtml.cs  —  PageModel for the Edit Task form
//
// Two handlers:
//   OnGet(int id)  → load the task from DB, pre-fill the form
//   OnPost()       → validate edits, save, redirect
//
// Key difference from Create:
//   Edit must load existing data on GET (Create starts blank).
//   Edit must pass the task Id through the form so POST knows
//   which row to UPDATE.
// ================================================================


namespace TeamDevelopment_team1.Pages.Todos
{
    public class EditModel : PageModel
    {
        private readonly ITodoRepository _repo;

        public EditModel(ITodoRepository repo)
        {
            _repo = repo;
        }

        // The form input fields (Title, Detail, DueDate, Priority)
        [BindProperty]
        public TodoInputModel Input { get; set; } = new TodoInputModel();

        // The Id of the task being edited.
        // [BindProperty] keeps this value in a hidden field so the POST
        // handler knows which row to UPDATE.
        [BindProperty]
        public int TodoId { get; set; }

        // Shown in the page header: "Editing: Buy groceries"
        public string TaskTitle { get; set; } = "";

        // ── OnGet ─────────────────────────────────────────────────────
        // The URL is /Todos/Edit/5  →  id = 5
        // We load that task from DB and pre-fill the form with its data.
        public IActionResult OnGet(int id)
        {
            // Reject obviously invalid Ids before touching the database
            // This handles URLs like /Todos/Edit/-1 or /Todos/Edit/0
            if (id <= 0)
            {
                return NotFound();   // returns HTTP 404
            }

            // Ask the repository for the task with this Id
            // GetById returns null if no row has that Id
            TodoItem? todo = _repo.GetById(id);

            // If null: the Id doesn't exist in the DB
            // This handles URLs like /Todos/Edit/99999
            if (todo == null)
            {
                return NotFound();   // returns HTTP 404
            }

            // Store the Id so it goes into the hidden field on the form
            TodoId = todo.Id;

            // Store the title for the page header
            TaskTitle = todo.Title;

            // Pre-fill the form with the existing values
            Input = new TodoInputModel
            {
                Title = todo.Title,
                Detail = todo.Detail,
                DueDate = todo.DueDate,
                Priority = todo.Priority
            };

            return Page();
        }

        // ── OnPost ────────────────────────────────────────────────────
        // Called when the user submits the edit form.
        // TodoId comes from the hidden field — it tells us which row to update.
        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Input.Title))
            {
                ModelState.AddModelError(
                    nameof(Input.Title),
                    "Task name cannot be blank.");
            }

            if (!ModelState.IsValid)
            {
                return Page();   // show form again with validation errors
            }

            TodoItem updatedTodo = new TodoItem
            {
                Id = TodoId,   // which row to UPDATE
                Title = Input.Title.Trim(),
                Detail = string.IsNullOrWhiteSpace(Input.Detail)
                               ? null
                               : Input.Detail.Trim(),
                DueDate = Input.DueDate,
                Priority = Input.Priority
            };

            try
            {
                _repo.Update(updatedTodo);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not update task: " + ex.Message);
                return Page();
            }
        }
    }
}