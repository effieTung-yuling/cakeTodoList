namespace cakeTodoList.Model
{
    public class Products
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public Boolean IsSale { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } // 自動產生的時間
    }

    public class UserProducts
    {
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
    }

    // 給 Put 用的：完整，包含所有可編輯狀態
    public class UpdateProductRequest
    {
        public string ProductName { get; set; }
        public double Price { get; set; }
        public bool IsSale { get; set; } // 這裡包含 IsSale
        public string Description { get; set; }
    }
}
