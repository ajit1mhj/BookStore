using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepo;
        public StockController(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        public async Task<IActionResult> Index(string sTerm = "")
        {
            var stocks = await _stockRepo.GetStocks(sTerm);
            return View(stocks);
        }
        public async Task<IActionResult> ManageStock(int bookId)
        {
            var existingStock = await _stockRepo.GetStockByBookIdAsync(bookId);
            var stock = new StockDTO
            {
                BookId = bookId,
                Quantity = existingStock != null ? existingStock.Quantity : 0
            };
            return View(stock);
        }
        [HttpPost]
        public async Task<IActionResult> ManageStock(StockDTO stock)
        {
            if (!ModelState.IsValid)
            {
                return View(stock);
            }
            try
            {
                await _stockRepo.ManageStock(stock);
                TempData["SuccessMessage"] = "Stock updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the stock.";
            
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
