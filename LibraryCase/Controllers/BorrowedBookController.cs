using Microsoft.AspNetCore.Mvc;
using LibraryCase.Application.Contracts.Books;
using LibraryCase.Application.Contracts.BorrowedBook;
using LibraryCase.Application.Validations;

namespace LibraryCase.Controllers
{
    public class BorrowedBookController : Controller
    {
        private readonly IBorrowedBookAppService _borrowedBookAppService;
        private readonly IBookAppService _bookAppService;

        public BorrowedBookController(IBorrowedBookAppService borrowedBookAppService, IBookAppService bookAppService)
        {
            _borrowedBookAppService = borrowedBookAppService;
            _bookAppService = bookAppService;
        }
        [HttpPost]
        public async Task<IActionResult> PostBorrowedBookAsync(BorrowedBookCreateDto postRequest)
        {
            var validationResult = new BorrowedBookValidator().Validate(postRequest);
            if (validationResult.IsValid)
                await _borrowedBookAppService.PostBorrowedBooksAsync(postRequest);
            else
            {
                foreach (var item in validationResult.Errors)
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                TempData.Keep("BookName");
                return View();
            }
            return RedirectToAction("Index", "Book");
        }

        public async Task<IActionResult> PostBorrowedBookAsync(Guid Id)
        {
            BookDto bookDto = (await _bookAppService.GetBookById(Id));
            TempData["BookName"] = bookDto.Name;
            TempData.Keep("BookName");
            return View(new BorrowedBookDto() { BookId = Id });
        }
        public async Task<IActionResult> PostReturnBookAsync(Guid Id, Guid BookId)
        {
            await _borrowedBookAppService.PostReturnBooksAsync(Id, BookId);
            return RedirectToAction("Index", "Book");
        }
    }
}
