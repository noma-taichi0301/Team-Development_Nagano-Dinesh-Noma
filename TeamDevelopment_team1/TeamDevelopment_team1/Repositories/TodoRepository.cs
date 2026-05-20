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
                    "DefaultConnection not found in appsettings.json");
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
        // 機能 1 — GetAll
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

    }
}
