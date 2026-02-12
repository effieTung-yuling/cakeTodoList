namespace cakeTodoList.Data
{
    using cakeTodoList.Model;
    using Microsoft.EntityFrameworkCore;
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Products> Products { get; set; }
    }
}
