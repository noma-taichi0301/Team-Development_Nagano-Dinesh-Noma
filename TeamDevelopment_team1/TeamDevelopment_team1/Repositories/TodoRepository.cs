using Microsoft.Data.SqlClient;

namespace TeamDevelopment_team1.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        // Stores the connection string from appsettings.json.
        // Example value:
        //   "Server=(localdb)\MSSQLLocalDB;Database=TodoAppDb;Trusted_Connection=True;"
        private readonly string _connectionString;

        // Constructor — ASP.NET Core calls this and passes IConfiguration.
        // IConfiguration reads appsettings.json automatically.
        public TodoRepository(IConfiguration config)
        {
            // GetConnectionString("DefaultConnection") reads the value from:
            //   appsettings.json → "ConnectionStrings" → "DefaultConnection"
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection not found in appsettings.json");
        }

        // ── Private helper: open a database connection ────────────────
        // Every public method calls this to get a ready connection.
        // "using var conn = OpenConnection()" means:
        //   - conn.Open() is called inside here
        //   - the "using" keyword closes and disposes the connection
        //     automatically when the method block ends
        private SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();   // actually connects to SQL Server right now
            return conn;
        }
    }
}
