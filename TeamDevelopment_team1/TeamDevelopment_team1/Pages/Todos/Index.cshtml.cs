using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeamDevelopment_team1.Models;
using TeamDevelopment_team1.Repositories;

namespace TeamDevelopment_team1.Pages.Todos
{
    public class IndexModel : PageModel
    {
        // _repo は ASP.NET Core によって注入されます (Program.cs で設定)。
        // PageModel は TodoRepository を自身で作成することはありません。
        // インターフェースを使用するだけなので、背後にあるクラスが何であるかは気にしません。
        private readonly ITodoRepository _repo;

        public IndexModel(ITodoRepository repo)
        {
            _repo = repo;
        }

        // ページ上のテーブルに表示されるタスクのリスト
        public List<TodoItem> Todos { get; set; } = new List<TodoItem>();

        // 上部の統計カードに表示される3つの数字
        public int StatTotal { get; set; }
        public int StatCompleted { get; set; }
        public int StatOverdue { get; set; }

        // [BindProperty(SupportsGet = true)] means:
        //   - on POST: the value comes from the form
        //   - on GET:  the value comes from the URL  (?Filter=incomplete)
        // This keeps the filter active after toggling or deleting.
        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        // ── OnGet ─────────────────────────────────────────────────────
        // Called every time the browser loads this page (GET request).
        // Fills Todos and the stat numbers, then Razor renders the view.
        public void OnGet()
        {
            try
            {
                // Call the repository — get back a List<TodoItem> directly
                Todos = _repo.GetAll(Filter);

                // Get the three stat numbers as a tuple
                var stats = _repo.GetStats();
                StatTotal = stats.Total;
                StatCompleted = stats.Completed;
                StatOverdue = stats.Overdue;
            }
            catch (Exception ex)
            {
                // If the DB is down or the table is missing, show a
                // friendly message instead of a crash page.
                ModelState.AddModelError("", "Could not load tasks: " + ex.Message);
                Todos = new List<TodoItem>();
            }
        }

        // ── OnPostToggle ──────────────────────────────────────────────
        // Called when the user clicks the toggle (complete/incomplete) button.
        // The form posts the task Id as a hidden field.
        // Returns IActionResult so we can redirect after saving.
        public IActionResult OnPostToggle(int id)
        {
            // Basic safety check — reject garbage values
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                _repo.ToggleStatus(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not update task: " + ex.Message);
            }

            // POST-Redirect-GET (PRG) pattern:
            // After a POST we always redirect back to the GET page.
            // This prevents "resubmit form?" if the user presses F5.
            // new { Filter } keeps the current filter in the URL.
            return RedirectToPage(new { Filter });
        }

        // ── OnPostDelete ──────────────────────────────────────────────
        // Called when the user confirms and submits the Delete form.
        public IActionResult OnPostDelete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                _repo.Delete(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete task: " + ex.Message);
            }

            return RedirectToPage(new { Filter });
        }
    }
}
