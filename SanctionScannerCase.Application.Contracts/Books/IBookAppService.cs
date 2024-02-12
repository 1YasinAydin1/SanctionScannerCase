using SanctionScannerCase.Application.Contracts.Results;

namespace SanctionScannerCase.Application.Contracts.Books
{
    public interface IBookAppService
    {
        /// <summary>
        /// Kitap bilgisi BooksCreateDto ile alınarak Database üzerine kayıt edilir.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<Response<string>> PostBooksAsync(BooksCreateDto postRequest);
        /// <summary>
        /// Kitap bilgileri BookJoinBorrowedBookDto ile  Database üzerinden dönülür.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Response<List<BookJoinBorrowedBookDto>> GetBooksAsync();
        /// <summary>
        /// Kitap bilgisi Id ile Database üzerinden dönülür.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<BookDto> GetBookById(Guid Id);
    }
}
