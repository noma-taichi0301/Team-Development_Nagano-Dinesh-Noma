using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using TeamDevelopment_team1.Models;
using static System.Net.WebRequestMethods;

namespace TeamDevelopment_team1.Pages
{
    public class IndexModel : PageModel
    {
        //private readonly string _connectionString;

        //// Dependency injection fetches connection parameters out of appsettings
        //public IndexModel(IConfiguration config)
        //{
        //    // Use a connection string value (replace "DefaultConnection" with your key as needed)
        //    _connectionString = config.GetConnectionString("DefaultConnection");

        //}

        //// Holds data queried out of the database to display inside our HTML loop
        //public List<TodoItem> ItemsList { get; set; } = new List<TodoItem>();
        //// Captures user textual data when appending new tasks
        //[BindProperty]
        //public string NewTodoTitle { get; set; }

        //public void OnGet()
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        var sql = "SELECT * FROM Todos ORDER BY Id ASC";
        //        connection.Open();
        //        ItemsList = connection.Query<TodoItem>(sql).ToList();
        //    }
        //}
    }
    
}
