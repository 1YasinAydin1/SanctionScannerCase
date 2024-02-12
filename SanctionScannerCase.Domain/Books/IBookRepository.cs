namespace SanctionScannerCase.Domain.Books
{
    public interface IBookRepository
    {
        /// <summary>
        /// Kitap bilgileri BookJoinBorrowedBook ile  Database üzerinden dönülür.
        /// </summary>
        /// <returns></returns>
        IQueryable<BookJoinBorrowedBook> GetBookNavigationAll();
        /// <summary>
        /// Kitap bilgisi Book ile alınarak Database üzerine kayıt edilir.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<bool> CreateBookAysnc(Book book);
        /// <summary>
        /// Kitap Ödünç Durumu bool alan ile  Database üzerine update edilir.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<bool> UpdateBookForBorrowedAysnc(Guid bookId,bool status);
        /// <summary>
        /// Kitap bilgisi Id ile Database üzerinden dönülür.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<Book> GetBookByIdAsync(Guid Id);
    }
}
