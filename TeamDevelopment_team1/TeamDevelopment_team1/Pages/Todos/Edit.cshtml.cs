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

        // フォーム入力フィールド (Title, Detail, DueDate, Priority)
        [BindProperty]
        public TodoInputModel Input { get; set; } = new TodoInputModel();

        //編集中のタスクのID。
        // [BindProperty] この値を非表示フィールドに保持するため、POST は
        //ハンドラーは更新すべき行を認識しています。
        [BindProperty]
        public int TodoId { get; set; }

        // ページヘッダーに表示されるタイトル: "Editing: Buy groceries"
        public string TaskTitle { get; set; } = "";

        // ──新機能：ドロップダウンリストのユーザー ───────────────────────────────────────────────
        public List<User> Users { get; set; } = new List<User>();

        // ── OnGet ─────────────────────────────────────────────────────
        // URL は /Todos/Edit/5  →  id = 5
        // データベースからそのタスクを読み込み、フォームにそのデータを事前に入力します。
        public IActionResult OnGet(int id)
        {
            // 無効な Id をデータベースに触れる前に拒否します
            // これにより /Todos/Edit/-1 や /Todos/Edit/0 のような URL を処理します
            if (id <= 0)
            {
                return NotFound();   // HTTP 404 を返します
            }

            // リポジトリにこの Id のタスクを問い合わせます
            // GetById は、その Id の行が存在しない場合は null を返します
            TodoItem? todo = _repo.GetById(id);

            // null の場合: DB にその Id は存在しません
            // これにより /Todos/Edit/99999 のような URL を処理します
            if (todo == null)
            {
                return NotFound();   // HTTP 404 を返します
            }

            // ── 新機能：ドロップダウンリストのユーザーを読み込む ──────────────────
            Users = _repo.GetAllUsers();

            // 非表示フィールドに Id を保持するため、フォームに埋め込みます
            TodoId = todo.Id;

            // ページヘッダーに表示されるタイトルを保持します
            TaskTitle = todo.Title;

            //既存の値でフォームを事前入力する
            Input = new TodoInputModel
            {
                Title = todo.Title,
                Detail = todo.Detail,
                DueDate = todo.DueDate,
                Priority = todo.Priority,
                AssigneeId = todo.AssigneeId
            };

            return Page();
        }

        // ── OnPost ────────────────────────────────────────────────────
        // ユーザーが編集フォームを送信したときに呼び出されます。
        // TodoId は非表示フィールドから取得されます — どの行を更新するかを示します。
        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Input.Title))
            {
                ModelState.AddModelError(
                    nameof(Input.Title),
                    "タスク名は空欄にすることはできません");
            }

            if (!ModelState.IsValid)
            {
                Users = _repo.GetAllUsers();    // ← NEW
                return Page();   // 検証エラーを含むフォームを再度表示します
            }

            TodoItem updatedTodo = new TodoItem
            {
                Id = TodoId,   // どの行を更新するか
                Title = Input.Title.Trim(),
                Detail = string.IsNullOrWhiteSpace(Input.Detail)
                               ? null
                               : Input.Detail.Trim(),
                DueDate = Input.DueDate,
                Priority = Input.Priority,
                // ──  0をnullに変換し、その他の値はそのまま保持します。 ──
                AssigneeId = (Input.AssigneeId == null || Input.AssigneeId == 0)
                     ? null
                     : Input.AssigneeId
            };

            try
            {
                _repo.Update(updatedTodo);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "タスクを更新できませんでした： " + ex.Message);
                Users = _repo.GetAllUsers();    // ← NEW
                return Page();
            }
        }
    }
}