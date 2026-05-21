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

        // [BindProperty(SupportsGet = true)] の意味:
        // - POST リクエストの場合: 値はフォームから取得されます
        // - GET リクエストの場合: 値は URL (?Filter=incomplete) から取得されます
        // これにより、切り替えや削除後もフィルタがアクティブな状態を維持します。
        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        // ── OnGet ─────────────────────────────────────────────────────
        // ブラウザがこのページを読み込むたびに呼び出されます（GETリクエスト）。
        // Todos と統計情報を取得し、Razor がビューをレンダリングします。
        public void OnGet()
        {
            try
            {
                // リポジトリを呼び出し、List<TodoItem> を直接取得します
                Todos = _repo.GetAll(Filter);

                // 3つの統計値をタプルとして取得
                var stats = _repo.GetStats();
                StatTotal = stats.Total;
                StatCompleted = stats.Completed;
                StatOverdue = stats.Overdue;
            }
            catch (Exception ex)
            {
                // DB がダウンしている場合やテーブルが存在しない場合、クラッシュページの代わりに
                // フレンドリーなメッセージを表示します。
                ModelState.AddModelError("", "タスクを読み込めませんでした: " + ex.Message);
                Todos = new List<TodoItem>();
            }
        }

         // ── OnPostToggle ──────────────────────────────────────────────
        // ユーザーが完了/未完了の切り替えボタンをクリックしたときに呼び出されます。
        // フォームはタスクIDを非表示フィールドとして送信します。
        // 保存後にリダイレクトできるように、IActionResultを返します。
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
                ModelState.AddModelError("", "タスクを更新できませんでした: " + ex.Message);
            }

            // POST-Redirect-GET (PRG) pattern:
            // POSTリクエストの後は、必ずGETページにリダイレクトされます。
            // これにより、ユーザーがF5キーを押した場合に「フォームを再送信しますか？」というメッセージが表示されるのを防ぎます。
            // new { Filter } は、現在のフィルターを URL に保持します。
            return RedirectToPage(new { Filter });
        }

        // ── OnPostDelete ──────────────────────────────────────────────
        // ユーザーが削除フォームを確認して送信したときに呼び出されます。
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
                ModelState.AddModelError("", "タスクを削除できませんでした: " + ex.Message);
            }

            return RedirectToPage(new { Filter });
        }
    }
}
