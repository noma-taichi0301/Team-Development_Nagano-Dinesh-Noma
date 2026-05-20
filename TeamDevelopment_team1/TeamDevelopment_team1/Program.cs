using TeamDevelopment_team1.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 依存性注入の設定: ITodoRepository を TodoRepository にマッピングします。
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();   // serve files from wwwroot/ (CSS, JS, images)
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

// Visiting "/" redirects automatically to the task list
app.MapGet("/", () => Results.Redirect("/Todos"));

app.Run();
