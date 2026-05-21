using Dapper;
using Microsoft.Data.SqlClient;
using TeamDevelopment_team1.Models;

namespace TeamDevelopment_team1.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        // appsettings.json から接続文字列を保存します。
        // 例:
        //   "Server=(localdb)\MSSQLLocalDB;Database=TodoAppDb;Trusted_Connection=True;"
        private readonly string _connectionString;

        // コンストラクター — ASP.NET Core がこれを呼び出し、IConfiguration を渡します。
        // IConfiguration は appsettings.json を自動的に読み取ります。
        public TodoRepository(IConfiguration config)
        {
            // GetConnectionString("DefaultConnection") は次の値を読み取ります:
            //   appsettings.json → "ConnectionStrings" → "DefaultConnection"
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "appsettings.json に DefaultConnection が見つかりません");
        }

        // ── Private helper: データベース接続を開く ────────────────
        // すべての public メソッドはこれを呼び出して準備済みの接続を取得します。
        // "using var conn = OpenConnection()" の意味:
        //   - conn.Open() はここで呼び出されます
        //   - "using" キーワードはメソッドブロックが終了すると自動的に接続を閉じて破棄します
        private SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();   // 実際にSQL Serverに接続しています
            return conn;
        }

        // ================================================================
        // 機能1 + 2 — すべて取得
        // ================================================================

        // TodoItem オブジェクトのリストを返します。
        // 各 TodoItem は Todos テーブルの 1 行に相当します。
        // 引数;filterによって WHERE 句が決定されます。

        public List<TodoItem> GetAll(string? filter)
        {
            // 接続を開始する — "using"で自動的に閉じられる
            using SqlConnection conn = OpenConnection();

            // SQLを構築する
            string sql;

            if (filter == "incomplete")
            {
                // 未完了を選択 IsCompleted = 0
                // 優先度の高い順、次に登録が新しい順でソートする
                sql = @"
                SELECT *
                FROM   Todos
                WHERE  IsCompleted = 0
                ORDER BY
                    CASE Priority
                        WHEN 3 THEN 1
                        WHEN 2 THEN 2
                        WHEN 1 THEN 3
                    END,
                    CreatedAt DESC";
            }
            else if (filter == "complete")
            {
                // 完了を選択 IsCompleted = 1
                sql = @"
                SELECT *
                FROM   Todos
                WHERE  IsCompleted = 1
                ORDER BY CreatedAt DESC";
            }
            else
            {
                // 全リスト表示 — 未完了タスクを最初に、次に優先度順にソート
                sql = @"
                SELECT *
                FROM   Todos
                ORDER BY
                    IsCompleted ASC,
                    CASE Priority
                        WHEN 3 THEN 1
                        WHEN 2 THEN 2
                        WHEN 1 THEN 3
                    END,
                    CreatedAt DESC";
            }

            //構築したSQLを実行
            List<TodoItem> todos = conn.Query<TodoItem>(sql).ToList();
            return todos;
        }

        // ================================================================
        // 機能 2 — GetById
        // ================================================================

        // 指定されたIDのタスクを返します。
        // 該当するIDがない場合はnullを返します。
        // 編集ページで選択されたタスクの内容表示に私用します。
        public TodoItem? GetById(int id)
        {
            // 接続を開始する — "using"で自動的に閉じられる
            using SqlConnection conn = OpenConnection();

            // @Id は Dapper のパラメータです。
            // Dapper は @Id を id の値で安全に置き換えます。
            // これにより SQL インジェクション攻撃を防ぎます。文字列連結は絶対に使用しないでください。
            string sql = "SELECT * FROM Todos WHERE Id = @Id";



            // QueryFirstOrDefault<TodoItem>:
            // - SQLを実行します
            // - 行が見つかった場合 → TodoItem に値を設定し、返します
            // - 行が見つからなかった場合 → null を返します（例外はスローしません）
            TodoItem? todos = conn.QueryFirstOrDefault<TodoItem>(sql, new { Id = id });
            return todos;
        }

        // ================================================================
        // 機能3 — 作成(CREATE)
        // ================================================================
        // Todosテーブルに新しい行を1つ挿入します。
        // 何も返さず、データベースに保存するだけです。
        public void Create(TodoItem todo)
        {
            using SqlConnection conn = OpenConnection();

            // Id は挿入しません — SQL が自動生成します (IDENTITY)
            // CreatedAt は挿入しません — SQL の DEFAULT GETDATE() が設定します
            // IsCompleted は新しいタスクでは常に 0 です
            string sql = @"
            INSERT INTO Todos (Title, Detail, DueDate, Priority, IsCompleted)
            VALUES            (@Title, @Detail, @DueDate, @Priority, 0)";

            // new { ... } は匿名オブジェクトです。
            // Dapper はそのプロパティを読み取り、SQL の @Parameters と照合します。
            // (int)todo.Priority は列挙値を int 型にキャストします。
            // Priority.High → 3
            // Priority.Mid → 2
            // Priority.Low → 1
            conn.Execute(sql, new
            {
                todo.Title,
                todo.Detail,
                todo.DueDate,
                Priority = (int)todo.Priority
            });
        }

        // ================================================================
        // / 機能 4 — 更新(UPDATE
        // ================================================================
        // 既存の行の編集可能な列を上書きします。
        // WHERE句「Id = @Id」により、変更される行は1つだけであることが保証されます。
        public void Update(TodoItem todo)
        {
            using SqlConnection conn = OpenConnection();

            // フォームでユーザーが編集できるフィールドのみを更新します。
            // IsCompleted はここには含まれません — トグルボタンが別途処理します。
            // CreatedAt is NOT here — it should never change.
            string sql = @"
            UPDATE Todos
            SET    Title    = @Title,
                   Detail   = @Detail,
                   DueDate  = @DueDate,
                   Priority = @Priority
            WHERE  Id = @Id";

            conn.Execute(sql, new
            {
                todo.Title,
                todo.Detail,
                todo.DueDate,
                Priority = (int)todo.Priority,
                todo.Id          // WHERE句「Id = @Id」に対応します
            });
        }

        // ================================================================
        // 機能5 — ステータス切り替え
        // ================================================================
        // 1つのSQL文でIsCompletedを0と1の間で反転します。
        // 最初に現在の値を読み取る必要はありません。
        public void ToggleStatus(int id)
        {
            using SqlConnection conn = OpenConnection();

            // ~ は SQL Server のビット単位の NOT 演算子です。
            // BIT 列では次のように動作します:
            //   現在の値 = 0  →  ~0 = 1  (タスクが完了としてマークされます)
            //   現在の値 = 1  →  ~1 = 0  (タスクが未完了としてマークされます)
            string sql = "UPDATE Todos SET IsCompleted = ~IsCompleted WHERE Id = @Id";

            conn.Execute(sql, new { Id = id });
        }

        // ================================================================
        // 機能6 — 削除
        // ================================================================
        // 行を完全に削除します。元に戻すことはできません。
        // ビューのJS confirm()ダイアログで「本当に削除しますか？」と確認します。
        public void Delete(int id)
        {
            using SqlConnection conn = OpenConnection();

            string sql = "DELETE FROM Todos WHERE Id = @Id";

            conn.Execute(sql, new { Id = id });
        }

        // ================================================================
        // GetStats州を取得する  
        // ================================================================
        // ダッシュボードの統計カード用に3つの数値を返します。
        // 名前付きタプルを使用することで、複数の値を返すことができます。
        // 別途クラスを作成する必要はありません。
        public (int Total, int Completed, int Overdue) GetStats()
        {
            using SqlConnection conn = OpenConnection();

            string sql = @"
            SELECT
                COUNT(*) AS Total,

                SUM(CAST(IsCompleted AS INT)) AS Completed,

                SUM(
                    CASE
                        WHEN IsCompleted = 0
                         AND DueDate     IS NOT NULL
                         AND CAST(DueDate AS DATE) < CAST(GETDATE() AS DATE)
                        THEN 1
                        ELSE 0
                    END
                ) AS Overdue

            FROM Todos";

            // QueryFirst は、1 行を動的オブジェクトとして返します。
            // 動的とは​​、.Total、.Completed、.Overdue を読み取ることができることを意味します。
            // それらのクラスを宣言する必要はありません。
            dynamic row = conn.QueryFirst(sql);

            int total = (int)(row.Total ?? 0);
            int completed = (int)(row.Completed ?? 0);
            int overdue = (int)(row.Overdue ?? 0);

            return (total, completed, overdue);
        }
    }
}