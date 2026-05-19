using Microsoft.Data.SqlClient;

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
    }
}
