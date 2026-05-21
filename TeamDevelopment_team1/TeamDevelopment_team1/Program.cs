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

app.UseStaticFiles();   // wwwroot/ からファイル（CSS、JS、画像）を配信する
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

// // "/" にアクセスすると、自動的にタスクリストにリダイレクトされます。
app.MapGet("/", () => Results.Redirect("/Todos"));

app.Run();
