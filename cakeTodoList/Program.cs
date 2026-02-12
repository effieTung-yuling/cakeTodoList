using cakeTodoList.Data;
using cakeTodoList.Repositories;
using cakeTodoList.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run("http://0.0.0.0:8080");
