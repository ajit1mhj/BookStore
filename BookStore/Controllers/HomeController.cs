using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using System.Runtime.CompilerServices;
using BookStore.Models.DTOS;

namespace BookStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHomeRepository _homeRepository;

    public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
    {
        _homeRepository = homeRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string sterm="",int genreId=0)
    {
        IEnumerable<Book> books = await _homeRepository.GetBooks(sterm,genreId);
        IEnumerable<Genre> genres = await _homeRepository.Genres();
        BookDisplayModel bookDisplay = new BookDisplayModel()
        {
            Books = books,
            Genres = genres,
            sTerm = sterm,
            GenreId = genreId

        };
        return View(bookDisplay);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
