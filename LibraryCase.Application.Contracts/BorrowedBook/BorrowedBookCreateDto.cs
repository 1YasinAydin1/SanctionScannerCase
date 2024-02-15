namespace LibraryCase.Application.Contracts.BorrowedBook
{
    public class BorrowedBookCreateDto
    {
        public DateTime BorrowDate { get; set; } = DateTime.Now.Date;
        public DateTime? ReturnDate { get; set; }
        public string? BorrowerTc { get; set; }
        public string? BorrowerName { get; set; }
        public Guid BookId { get; set; }
    }
}
