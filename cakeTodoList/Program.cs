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
// --- 2. 啟用 CORS (必須放在 Map 之前) ---
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//    app.MapScalarApiReference(options =>
//    {
//        options.WithTitle("我的 API 文件")
//               .WithTheme(ScalarTheme.Moon);
//    });
//}
// 無論是在本機還是 Zeabur，都啟用 OpenAPI 和 Scalar
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("我的 API 文件")
           .WithTheme(ScalarTheme.Moon)
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient); // 選配：設定預設客戶端
});
// 在 Zeabur 建議先註解掉，避免 Redirect 導致的連線問題
// app.UseHttpsRedirection(); 

app.UseAuthorization();
app.MapControllers();

// Zeabur 預設通常監聽 8080，這行保留是正確的
app.Run("http://0.0.0.0:8080");
