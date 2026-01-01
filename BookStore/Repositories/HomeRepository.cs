using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm?.Trim().ToLower();

            var query =
                from book in _db.Books
                join genre in _db.Genres
                    on book.GenreId equals genre.Id
                join stock in _db.Stocks
                    on book.Id equals stock.BookId into bookStocks
                from bookWithStock in bookStocks.DefaultIfEmpty()
                where (string.IsNullOrWhiteSpace(sTerm)
                       || EF.Functions.Like(book.BookName.ToLower(), sTerm + "%"))
                select new Book
                {
                    Id = book.Id,
                    Image = book.Image,
                    AuthorName = book.AuthorName,
                    BookName = book.BookName,
                    GenreId = book.GenreId,
                    Price = book.Price,
                    GenreName = genre.Name,
                    Quantity = bookWithStock == null ? 0 : bookWithStock.Quantity
                };

            if (genreId > 0)
            {
                query = query.Where(b => b.GenreId == genreId);
            }

            return await query.ToListAsync();
        }

    }
}
