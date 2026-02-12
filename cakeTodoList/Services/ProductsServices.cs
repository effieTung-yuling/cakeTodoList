using cakeTodoList.Model;
using cakeTodoList.Repositories;

namespace cakeTodoList.Services
{
    public class ProductsServices
    {
        private readonly ProductsRepositories _repo;// 你只有宣告，但「沒有人」給它值

        // 你少了建構子來接住 DI 注入進來的實例！
        public ProductsServices(ProductsRepositories repo)
        {
            _repo = repo;
        }
        public List<Products> GetDiscountedProducts()
        {
            var rawData = _repo.GetDiscountedProducts();

            // 如果資料庫連線失敗或回傳 null，先給一個空清單，避免後續 Select 崩潰
            if (rawData == null) return new List<Products>();
            return _repo.GetDiscountedProducts().Select(p => new Products
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Price = p.Price > 50 ? (int)(p.Price * 0.9) : p.Price, // 只在回傳時計算，不改原始清單 
                IsSale = p.IsSale,        // 補上這行，GET 才會顯示正確的狀態
                Description = p.Description,
                CreatedAt = p.CreatedAt
            }).ToList();
        }
        public List<Products> CreateProduct(Products product)
        {
            // 商業邏輯範例：價格不能為負數
            if (product.Price < 0)
            {
                throw new Exception("價格不能低於 0 元");
            }
            _repo.Add(product);
            return GetDiscountedProducts();
        }


        public void UpdateProduct(Products product)
        {
            // 這裡可以寫一些更新前的商業邏輯 (例如：結案的產品不能修改價格)
            // 商業邏輯：如果要調漲超過 1000 元需要額外權限 (此處僅為範例)
            if (product.Price > 10000)
            {
                throw new Exception("價格異常高，請聯繫經理。");
            }

            _repo.Update(product);
        }

        public Products GetProductById(int id)
        {
            return _repo.GetById(id);
        }
        public bool DeleteProduct(int id)
        {
            return _repo.Delete(id);
        }


    }
}
