using TeamDevelopment_team1.Models;

namespace TeamDevelopment_team1.Repositories;

public interface ITodoRepository
{
    List<TodoItem> GetAll(string? filter);

    TodoItem? GetById(int id);

    void Create(TodoItem todo);

    void Update(TodoItem todo);

    void ToggleStatus(int id);

    void Delete(int id);

    //(int Total, int Completed, int Overdue) GetStats();
}
