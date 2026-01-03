using BookStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IFileService _fileService;

        public BookController(IBookRepository bookRepository,
            IGenreRepository genreRepository,
            IFileService fileService)
        {
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            var books = await _bookRepository.GetBooks();
            return View(books);
        }
        public async Task<IActionResult> AddBook()
        {
            var genreSelectList = (await _genreRepository.GetAllGenres()).Select(Genre => new SelectListItem
            {
                Text = Genre.Name,
                Value = Genre.Id.ToString()
            });
            BookDTO bookToAdd = new()
            {
                GenreList = genreSelectList
            };
            return View(bookToAdd);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookDTO bookToAdd)
        {
            var genreSelectList = (await _genreRepository.GetAllGenres()).Select(Genre => new SelectListItem
            {
                Text = Genre.Name,
                Value = Genre.Id.ToString()
            });
            bookToAdd.GenreList = genreSelectList;
            if (!ModelState.IsValid)
                return View(bookToAdd);
            try
            {
                if (bookToAdd.ImageFile != null)
                {
                    if (bookToAdd.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image size should be less than 1MB");

                    }
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    string imageName = await _fileService.SaveFile(bookToAdd.ImageFile, allowedExtensions);
                    bookToAdd.Image = imageName;
                }
                Book book = new()
                {
                    Id = bookToAdd.Id,
                    BookName = bookToAdd.BookName,
                    AuthorName = bookToAdd.AuthorName,
                    Price = bookToAdd.Price,
                    Image = bookToAdd.Image,
                    GenreId = bookToAdd.GenreId
                };
                await _bookRepository.AddBook(book);
                TempData["SuccessMessage"] = "Book added successfully";
                return RedirectToAction(nameof(AddBook));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(bookToAdd);

            }
            catch (FileNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(bookToAdd);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the book.";
                return View(bookToAdd);
            }
        }

        public async Task<IActionResult> UpdateBook(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book == null)
            {

                TempData["ErrorMessage"] = $"Book with Id {id} not found";
                return RedirectToAction(nameof(Index));
            }

            var genreSelectList = (await _genreRepository.GetAllGenres()).Select(Genre => new SelectListItem
            {
                Text = Genre.Name,
                Value = Genre.Id.ToString(),
                Selected = Genre.Id == book.GenreId
            });
            BookDTO bookToUpdate = new()
            {
                Id = book.Id,
                BookName = book.BookName,
                AuthorName = book.AuthorName,
                Price = book.Price,
                Image = book.Image,
                GenreId = book.GenreId,
                GenreList = genreSelectList
            };
            return View(bookToUpdate);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBook(BookDTO bookToUpdate)
        {
            var genreSelectList = (await _genreRepository.GetAllGenres()).Select(Genre => new SelectListItem
            {
                Text = Genre.Name,
                Value = Genre.Id.ToString(),
                Selected = Genre.Id == bookToUpdate.GenreId
            });
            bookToUpdate.GenreList = genreSelectList;
            if (!ModelState.IsValid)
                return View(bookToUpdate);
            try
            {
                string oldImage = "";
                if (bookToUpdate.ImageFile != null)
                {
                    if (bookToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image size should be less than 1MB");
                    }
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    string imageName = await _fileService.SaveFile(bookToUpdate.ImageFile, allowedExtensions);
                    oldImage = bookToUpdate.Image;
                    bookToUpdate.Image = imageName;
                }
                Book book = new()
                {
                    Id = bookToUpdate.Id,
                    BookName = bookToUpdate.BookName,
                    AuthorName = bookToUpdate.AuthorName,
                    Price = bookToUpdate.Price,
                    Image = bookToUpdate.Image,
                    GenreId = bookToUpdate.GenreId
                };
                await _bookRepository.UpdateBook(book);
                if (!string.IsNullOrWhiteSpace(oldImage))
                {
                    _fileService.DeteleFile(oldImage);
                }
                TempData["SuccessMessage"] = "Book is updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(bookToUpdate);
            }
            catch (FileNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(bookToUpdate);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the book.";
                return View(bookToUpdate);
            }
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _bookRepository.GetBookById(id);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"Book with Id {id} not found";
                }
                else
                {
                    await _bookRepository.DeleteBook(book);
                    if (!string.IsNullOrWhiteSpace(book.Image))
                    {
                        _fileService.DeteleFile(book.Image);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on deleting the data";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
