namespace LibraryCase.Application.Contracts.BorrowedBook
{
    public class BorrowedBookGetDto
    {
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string BorrowerTc { get; set; }
        public string BorrowerName { get; set; }
        public Guid BookId { get; set; }
    }
}
