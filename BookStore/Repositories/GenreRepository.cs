using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories
{
    public interface IGenreRepository
    {
        Task AddGenre(Genre genre);
        Task  UpdateGenre(Genre genre);
        Task  DeleteGenre(Genre genre);
        Task<Genre?> GetGenreById(int Id);
        Task<IEnumerable<Genre>> GetAllGenres();
    }

    public class GenreRepositry : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepositry(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddGenre(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGenre(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGenre(Genre genre)
        {
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }
        public async Task<Genre?> GetGenreById(int Id)
        {
            return await _context.Genres.FindAsync(Id);
        }
        public async Task<IEnumerable<Genre>> GetAllGenres()
        {
            return await _context.Genres.ToListAsync();
        }
    }
}
