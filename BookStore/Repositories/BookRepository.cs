using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories
{
    public interface IBookRepository
    {
        Task AddBook(Book book);
        Task DeleteBook(Book book);
        Task<Book?> GetBookById(int id);
        Task<IEnumerable<Book>> GetBooks();

        Task UpdateBook(Book book);
    }
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _db;
        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddBook(Book book)
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteBook(Book book)
        {
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
        }

        public async Task<Book?> GetBookById(int id)=>
        
             await _db.Books.FindAsync(id);
        

        public  async Task<IEnumerable<Book>> GetBooks()=>
            await _db.Books.Include(a=>a.Genre).ToListAsync();
        public async Task UpdateBook(Book book)
        {
            _db.Books.Update(book);
            await  _db.SaveChangesAsync();
        }
    }
}
