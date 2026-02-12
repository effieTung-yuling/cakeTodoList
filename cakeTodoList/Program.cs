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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=MyCakeShop.db"));

builder.Services.AddScoped<ProductsRepositories>();
builder.Services.AddScoped<ProductsServices>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// --- 關鍵順序 1：CORS 必須放在最前面 ---
app.UseCors("AllowAll");

// --- 關鍵順序 2：啟用 API 文件 ---
app.MapOpenApi();
app.MapScalarApiReference(options => {
    options.WithTitle("我的 API 文件")
           .WithTheme(ScalarTheme.Moon);
    // 移除會報錯的 .WithServers(...)
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
