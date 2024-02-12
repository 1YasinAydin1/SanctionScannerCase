using SanctionScannerCase.Application.Contracts.BorrowedBook;

namespace SanctionScannerCase.Application.Contracts.Books
{
    public class BookJoinBorrowedBookDto
    {
        public Guid Id{ get; set; }
        public Guid BorrowedId { get; set; }
        public string Name { get; set; }
        public int PageCount { get; set; }
        public byte[] Image { get; set; }
        public string Author { get; set; }
        public bool Status { get; set; }
        public string Type { get; set; }
        public string CoverLetter { get; set; }
        public BorrowedBookDto BorrowedBooks { get; set; }
        //public DateTime BorrowDate { get; set; }
        //public DateTime ReturnDate { get; set; }
        //public string BorrowerTc { get; set; }
        //public string BorrowerName { get; set; }
    }
}
