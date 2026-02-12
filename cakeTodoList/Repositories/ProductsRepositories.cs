using cakeTodoList.Data;
using cakeTodoList.Model;
using Microsoft.EntityFrameworkCore;

namespace cakeTodoList.Repositories
{
    public class ProductsRepositories
    {
        private readonly AppDbContext _context;
        public ProductsRepositories(AppDbContext context)
        {
            _context = context;
        }
        // 改成去資料庫拿所有產品
        public List<Products> GetDiscountedProducts() => _context.Products.ToList();

        // 改成把產品存進資料庫
        public void Add(Products product)
        {
            _context.Products.Add(product);
            _context.SaveChanges(); // 這行等於按下「存檔鍵」，沒寫這行不會存進去
        }

        public void Update(Products product)
        {
            // 在 EF Core 中，直接使用 _context.Products
            _context.Products.Update(product);
            _context.SaveChanges();
        }
        public Products GetById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public bool Delete(int id) // 只需要 id，不需傳入 product
        {
            // 1. 找到那筆資料
            var target = _context.Products.FirstOrDefault(p => p.Id == id);

            if (target != null)
            {
                // 2. 執行移除並存檔
                _context.Products.Remove(target);
                _context.SaveChanges(); // 記得要存檔，不然資料庫不會動
                return true;
            }

            return false;
        }


    }
}
