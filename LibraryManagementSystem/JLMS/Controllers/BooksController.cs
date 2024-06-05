using JLMS.Data; // Ensure you have this namespace for ApplicationDbContext
using JLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JLMS.Controllers
{
    public class BooksController : Controller
    {
        private readonly ILogger<BooksController> _logger;
        private readonly ApplicationDbContext _context;

        public BooksController(ILogger<BooksController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
