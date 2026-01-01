using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        public GenreController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }
        public async Task<IActionResult> Index()
        {
            var genres = await _genreRepository.GetAllGenres();
            return View(genres);
        }

        public IActionResult AddGenre()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddGenre(GenreDTO genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre);
            }
            try
            {
                var GenreToAdd = new Genre
                {
                    Name = genre.GenreName,
                    Id = genre.Id
                };
                await _genreRepository.AddGenre(GenreToAdd);
                TempData["successMessage"] = "Genre added successfully";
                return RedirectToAction(nameof(AddGenre));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Genre Count not be added! ";
                return View(genre);
            }
        }

        public async Task<IActionResult> UpdateGenre(int id)
        {
            var genre = await _genreRepository.GetGenreById(id);
            if (genre is null)
            {
                throw new InvalidOperationException($"Genre with Id {id} not found");
            }
            var GenreToUpdate = new GenreDTO
            {
                Id = genre.Id,
                GenreName = genre.Name
            };
            return View(GenreToUpdate);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateGenre(GenreDTO genreYoUpdate)
        {
            if (!ModelState.IsValid)
            {
                return View(genreYoUpdate);
            }
            try
            {
                var genre = new Genre
                {
                    Name = genreYoUpdate.GenreName,
                    Id = genreYoUpdate.Id
                };
                await _genreRepository.UpdateGenre(genre);
                TempData["successMessage"] = "Genre updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Genre Count not be updated! ";
                return View(genreYoUpdate);
            }

        }
        public async Task<IActionResult> DeleteGenre(int id)
        {
            try
            {
                var genre = await _genreRepository.GetGenreById(id);
                if (genre is null)
                {
                    throw new InvalidOperationException($"Genre with Id {id} not found");
                }
                await _genreRepository.DeleteGenre(genre);
                TempData["successMessage"] = "Genre deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Genre Count not be deleted! ";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
