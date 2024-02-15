using Microsoft.AspNetCore.Mvc;
using LibraryCase.Application.Contracts.Books;
using LibraryCase.Application.Validations;
using X.PagedList;

namespace LibraryCase.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookAppService _bookAppService;
        private readonly ILogger<BookController> _logger;
        public BookController(ILogger<BookController> logger, IBookAppService bookAppService)
        {
            _logger = logger;
            _bookAppService = bookAppService;
        }

        public IActionResult Index(int page = 1, int pageSize = 4)
        {
            var list = _bookAppService.GetBooksAsync();
            return View(list.Data.ToPagedList(page, pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> PostBooksAsync(IFormFile imageFile, BooksCreateDto postRequest)
        {
            ViewBag.ImageError = false;
            if (imageFile == null || imageFile.Length == 0)
                ViewBag.ImageError = true;
            else
                using (MemoryStream ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    byte[] imageBytes = ms.ToArray();
                    postRequest.Image = imageBytes;
                }

            BookValidator validationRules = new BookValidator();
            var validationResult = validationRules.Validate(postRequest);
            if (validationResult.IsValid)
                await _bookAppService.PostBooksAsync(postRequest);
            else
            {
                foreach (var item in validationResult.Errors)
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                return View();
            }
            return RedirectToAction("Index", "Book");
        }
        public IActionResult PostBooksAsync()
        {
            ViewBag.ImageError = false;
            return View();
        }

    }
}