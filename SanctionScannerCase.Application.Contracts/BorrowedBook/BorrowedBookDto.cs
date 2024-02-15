using LibraryCase.Application.Contracts.Books;

namespace LibraryCase.Application.Contracts.BorrowedBook
{
    public class BorrowedBookDto
    {
        public Guid Id { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string BorrowerTc { get; set; }
        public string BorrowerName { get; set; }
        public Guid BookId { get; set; }
        public BookDto Book { get; set; }
    }
}
