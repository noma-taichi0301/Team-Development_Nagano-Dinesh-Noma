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

        // [BindProperty] ASP.NET Coreにこのオブジェクトを埋めるように指示します
        // ユーザーがフォームを送信した際に、フォームフィールドから取得されます。
        // 各フォームフィールド名はプロパティパスと一致する必要があります。
        //   <input name="Input.Title" />  →  Input.Title
        [BindProperty]
        public TodoInputModel Input { get; set; } = new TodoInputModel();

        // ── 新機能：ドロップダウンリストのユーザー一覧 ───────────────
        public List<User> Users { get; set; } = new List<User>();

        // ── OnGet ─────────────────────────────────────────────────────
        // ブラウザが「タスクを追加」をクリックしたときにGETリクエストを送信します。
        // 空のフォームをレンダリングするだけで、DBから読み込む必要はありません。
        public IActionResult OnGet()
        {
            // ── 新機能：ユーザーを読み込むことで、ドロップダウンリストにオプションが表示されるようになります ───
            Users = _repo.GetAllUsers();
            return Page();
        }

        // ── OnPost ────────────────────────────────────────────────────
        // ブラウザがフォームを送信したときにPOSTリクエストが送信されます。
        // 手順: 検証 → TodoItemの作成 → 保存 → リダイレクト
        public IActionResult OnPost()
        {
            // 追加のチェック: [Required] は null をキャッチしますが、空白のみはキャッチしません
            // 例: タイトルが "     " (5つのスペース) の場合、[Required] は通過します
            // そのため、この手動チェックを追加します
            if (string.IsNullOrWhiteSpace(Input.Title))
            {
                ModelState.AddModelError(
                    nameof(Input.Title),
                    "タスク名は空白またはスペースのみでは入力できません。");
            }

            // いずれかの検証属性が失敗した場合、ModelState.IsValid は false になります
            // （[Required]、[StringLength]、または上で行った手動チェックのいずれか）
            if (!ModelState.IsValid)
            {
                // ── NEW: reload users on validation failure ───
                Users = _repo.GetAllUsers();
                // 同じページを返します — Razor はエラーメッセージを表示します
                return Page();
            }

            // フォームデータから TodoItem を作成します
            TodoItem todo = new TodoItem
            {
                Title = Input.Title.Trim(),
                Detail = string.IsNullOrWhiteSpace(Input.Detail)
                               ? null
                               : Input.Detail.Trim(),
                DueDate = Input.DueDate,
                Priority = Input.Priority,
                
                AssigneeId = (Input.AssigneeId == null || Input.AssigneeId == 0)
                 ? null
                 : Input.AssigneeId
            };

            try
            {
                // データベースに保存します — 同期的（await なし）
                _repo.Create(todo);

                // 保存に成功したら一覧ページへリダイレクトします
                // （PRG パターン: F5 による重複送信を防ぎます）
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // 例外でクラッシュさせず、フォーム上にエラーを表示します
                ModelState.AddModelError("", "タスクを保存できませんでした: " + ex.Message);
                Users = _repo.GetAllUsers();    // ← NEW: reload on error
                return Page();
            }
        }
    }
}
