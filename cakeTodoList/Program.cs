using cakeTodoList.Data;
using cakeTodoList.Repositories;
using cakeTodoList.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. 新增 CORS 服務設定 ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=MyCakeShop.db"));

builder.Services.AddScoped<ProductsRepositories>();
builder.Services.AddScoped<ProductsServices>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// --- 重要修正 A：CORS 必須放在所有 Map 動作之前 ---
app.UseCors("AllowAll");

// --- 重要修正 B：明確指定 Scalar 的 Server 網址 ---
app.MapOpenApi();
app.MapScalarApiReference(options => {
    options.WithTitle("我的 API 文件")
           .WithTheme(ScalarTheme.Moon)
           // 這裡強制讓 Scalar 知道 API 在哪，避免它去連 localhost
           .WithServers([new ScalarServer("https://caketodolist.zeabur.app")]); 
});

// app.UseHttpsRedirection(); // 保持註解

app.UseAuthorization();
app.MapControllers();

// --- 自動建立資料表邏輯 ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated(); 
        Console.WriteLine("資料庫與資料表已成功確認/建立");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"建立資料庫時發生錯誤: {ex.Message}");
    }
}

app.Run("http://0.0.0.0:8080");
