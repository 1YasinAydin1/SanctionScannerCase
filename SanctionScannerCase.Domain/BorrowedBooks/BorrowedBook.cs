using SanctionScannerCase.Domain.Books;
using SanctionScannerCase.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanctionScannerCase.Domain.BorrowedBooks
{
    public class BorrowedBook : BaseEntity
    {
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime ActualReturnDate { get; set; }
        public string BorrowerTc { get; set; }
        public string BorrowerName { get; set; }

        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Book Book { get; set; }
    }
}
