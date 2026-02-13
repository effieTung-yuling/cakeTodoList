using cakeTodoList.Model;
using cakeTodoList.Services;
using Microsoft.AspNetCore.Mvc;

namespace cakeTodoList.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductsController:ControllerBase
    {
        private readonly ProductsServices _service;

        // 這裡絕對不能用 = new ProductsServices()，要用建構子注入
        public ProductsController(ProductsServices service)
        {
            _service = service;
        }
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var result = _service.GetDiscountedProducts();

                // 沒資料不代表報錯，回傳空清單 (200 OK) 是合理的
                // 但如果你想強調「找不到」，可以回傳 NotFound()
                if (result == null || !result.Any()) return NotFound("目前沒有產品資料");

                return Ok(result);
            }
            catch (Exception ex)
            {
                // 這裡可以攔截那些「未設置為物件實例」的低級錯誤
                return StatusCode(500, $"伺服器內部錯誤：{ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] UserProducts request)
        {
            if (request == null) return BadRequest();

            // 注意看這裡：要把 request (UserProducts) 轉成 product (Products)
            var product = new Products // 修正點：這裡要用資料庫對應的實體類別
            {
                // Id 不用寫，資料庫會自動跳號
                ProductName = request.ProductName,
                Price = request.Price,
                IsSale = true,        // 記得也要把 IsSale 傳過去
                Description = request.Description,
                CreatedAt = DateTime.UtcNow        // 可以在這裡設定初始時間
            };

            try
            {
                // 現在傳入的是 Products 類型，就不會報錯了！
                _service.CreateProduct(product);
                return Ok(new { message = "新增成功", data = product });
            }
            catch (Exception ex)
            {
                var realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"儲存失敗：{realError}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UpdateProductRequest request) // 1. 接收 DTO
        {
            if (request == null) return BadRequest();

            // 2. 先去資料庫確認這筆資料是否存在
            // 建議 Service 要提供一個 GetById 的方法，直接拿單筆效能更好
            var existingProduct = _service.GetProductById(id);
            if (existingProduct == null)
            {
                return NotFound($"找不到 ID 為 {id} 的產品");
            }

            // 3. 將 DTO 的新資料「搬運」到撈出來的 Entity 物件上
            existingProduct.ProductName = request.ProductName;
            existingProduct.Price = request.Price;
            existingProduct.IsSale = request.IsSale;
            existingProduct.Description = request.Description;

            try
            {
                // 4. 呼叫 Service 執行更新
                _service.UpdateProduct(existingProduct);
                return Ok(new { message = "修改成功", data = existingProduct });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"修改失敗：{ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var isSuccess = _service.DeleteProduct(id);

            if (!isSuccess)
            {
                return NotFound($"刪除失敗，找不到 ID 為 {id} 的產品");
            }

            return Ok(new { message = $"ID 為 {id} 的產品已成功刪除" });
        }

    }
}
