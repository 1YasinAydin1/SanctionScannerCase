using LibraryCase.Domain.BorrowedBooks;

namespace LibraryCase.Domain.Books
{
    public class BookJoinBorrowedBook
    {
        public Guid Id { get; set; }
        public Guid BorrowedId { get; set; }
        public string Name { get; set; }
        public int PageCount { get; set; }
        public byte[] Image { get; set; }
        public string Author { get; set; }
        public bool Status { get; set; }
        public string Type { get; set; }
        public string CoverLetter { get; set; }
        public BorrowedBook? BorrowedBooks { get; set; }
    }
}
