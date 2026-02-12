using cakeTodoList.Data;
using cakeTodoList.Repositories;
using cakeTodoList.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
// 1. 安裝冷藏庫 (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=MyCakeShop.db"));

// 2. 招募員工 (註冊三層架構)
builder.Services.AddScoped<ProductsRepositories>(); // 倉庫管理員
builder.Services.AddScoped<ProductsServices>();    // 主廚
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// 1. 啟用 OpenAPI 和 Scalar
app.MapOpenApi();
app.MapScalarApiReference(options => {
    options.WithTitle("我的 API 文件").WithTheme(ScalarTheme.Moon);
});

// 2. 啟用 CORS (重要！)
app.UseCors("AllowAll");

// 3. 註解掉這行 (解決 Log 中的紅字錯誤)
// app.UseHttpsRedirection(); 

app.UseAuthorization();
app.MapControllers();

// --- 修正：自動建立資料表邏輯 ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // 這行會檢查資料庫，如果沒資料表會自動根據 Model 建立
        context.Database.EnsureCreated(); 
        Console.WriteLine("資料庫與資料表已成功確認/建立");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"建立資料庫時發生錯誤: {ex.Message}");
    }
}

// 確保這行在最後
app.Run("http://0.0.0.0:8080");
