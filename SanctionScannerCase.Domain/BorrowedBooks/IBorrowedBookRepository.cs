namespace SanctionScannerCase.Domain.BorrowedBooks
{
    public interface IBorrowedBookRepository
    {
        /// <summary>
        /// Kitap bilgileri BorrowedBook ile  Database üzerinden dönülür.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<List<BorrowedBook>> GetBorrowedBookAllAsync();
        /// <summary>
        /// Kitap İade Etmek için Ödünç Durumu bool alan ile Database üzerine update edilir.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<bool> UpdateBorrowedBokForReturnAysnc(Guid Id);
        /// <summary>
        /// Ödünç Kitap bilgisi BorrowedBook ile alınarak Database üzerine kayıt edilir.
        /// </summary>
        /// <param name="postRequest"></param>
        /// <returns></returns>
        Task<bool> CreateBorrowedBookAysnc(BorrowedBook book);
    }
}
