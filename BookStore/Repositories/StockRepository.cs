using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;
        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Stock?> GetStockByBookIdAsync(int bookId) =>
           await _context.Stocks.FirstOrDefaultAsync(s => s.BookId == bookId);
        
        public async Task ManageStock(StockDTO StockToManage)
        {
            var existingStock = await GetStockByBookIdAsync(StockToManage.BookId);
            if (existingStock is null)
            {
                var stock = new Stock
                {
                    BookId = StockToManage.BookId,
                    Quantity = StockToManage.Quantity
                };
                _context.Stocks.Add(stock);

            }
            else
            {
                existingStock.Quantity = StockToManage.Quantity;
                
            }
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "")
        {
            var stocks = await (from book in _context.Books
                                join stock in _context.Stocks 
                                on book.Id equals stock.BookId
                                into book_stock
                                from bookStock in book_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) || book.BookName.ToLower().Contains(sTerm.ToLower())
                                select new StockDisplayModel
                                {
                                    BookId = book.Id,
                                    BookName = book.BookName,
                                    Quantity = bookStock == null ? 0: bookStock.Quantity

                                }).ToListAsync();
            return stocks;
        }

        
    }
    public interface IStockRepository
    {
        Task<Stock?> GetStockByBookIdAsync(int bookId);
        Task ManageStock(StockDTO StockToManage);
        Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "");
    }
}
