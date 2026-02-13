using cakeTodoList.Data;
using cakeTodoList.Repositories;
using cakeTodoList.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 註冊 CORS 服務
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 修正點：使用 builder.Environment 而不是 app.Environment ---
if (builder.Environment.IsDevelopment())
{
    // 本機開發：使用 SQLite
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=dev.db"));
}
else
{
    // 雲端環境 (Zeabur)：使用 PostgreSQL
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<ProductsRepositories>();
builder.Services.AddScoped<ProductsServices>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// --- 順序 1：CORS ---
app.UseCors("AllowAll");

// --- 順序 2：API 文件 ---
app.MapOpenApi();
app.MapScalarApiReference(options => {
    options.WithTitle("我的 API 文件")
           .WithTheme(ScalarTheme.Moon);
});

app.UseAuthorization();
app.MapControllers();

// 自動建立資料表
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        // 這裡的小優化：根據環境顯示不同的 Log
        string dbType = app.Environment.IsDevelopment() ? "SQLite" : "PostgreSQL";
        Console.WriteLine($"{dbType} 資料表已成功確認/建立");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"資料庫初始化失敗: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.Run(); // 本機會自動抓 launchSettings.json 裡的 Port (通常是 5xxx)
}
else
{
    app.Run("http://0.0.0.0:8080"); // 雲端用 8080
}