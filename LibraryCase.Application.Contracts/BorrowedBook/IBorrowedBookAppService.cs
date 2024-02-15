using LibraryCase.Application.Contracts.Results;

namespace LibraryCase.Application.Contracts.BorrowedBook
{
    public interface IBorrowedBookAppService
    {
        /// <summary>
        /// Ödünç Alınacak Kitap bilgisi BorrowedBookCreateDto ile alınarak Database üzerine kayıt edilir.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<Response<string>> PostBorrowedBooksAsync(BorrowedBookCreateDto postRequest);
        /// <summary>
        /// İade Edilecek Kitap bilgisi Id ve BookId ile alınarak Database üzerine kayıt edilir.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<Response<string>> PostReturnBooksAsync(Guid Id, Guid BookId);
    }
}
