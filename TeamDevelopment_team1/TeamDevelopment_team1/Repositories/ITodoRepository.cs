using TeamDevelopment_team1.Models;

namespace TeamDevelopment_team1.Repositories;

public interface ITodoRepository
{
    // 機能 1 + 2 — ステータスでフィルタリングされたすべてのタスクを返します
    // filter = null または "" → すべてのタスク
    // filter = "incomplete" → IsCompleted = 0 のタスクのみ
    // filter = "complete" → IsCompleted = 1 のタスクのみ
    List<TodoItem> GetAll(string? filter);

    // 機能4 — IDで1つのタスクを返します。見つからない場合はnullを返します。
    TodoItem? GetById(int id);

    // 機能3 — データベースに新しいタスクを挿入する
    void Create(TodoItem todo);

    // 機能4 — 既存のタスクへの編集内容を保存する
    void Update(TodoItem todo);


    // 機能 5 — IsCompleted を 0→1 または 1→0 に反転します
    void ToggleStatus(int id);

    // 機能6 — タスクを完全に削除する
    void Delete(int id);

    // ページ上部の統計カード用の3つの数値を返します
    (int Total, int Completed, int Overdue) GetStats();
}
