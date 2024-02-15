using FluentValidation;
using LibraryCase.Application.Contracts.BorrowedBook;

namespace LibraryCase.Application.Validations
{
    public class BorrowedBookValidator : AbstractValidator<BorrowedBookCreateDto>
    {
        public BorrowedBookValidator()
        {
            RuleFor(i => i.BorrowerTc).NotEmpty().NotNull().Length(11).Must(ValidateTCNumber).WithMessage("Geçersiz TC Kimlik Numarası.");
            RuleFor(i => i.BorrowerName).NotEmpty();
            RuleFor(i => i.ReturnDate).NotEmpty();
            RuleFor(i => i.BorrowDate).NotEmpty();
            RuleFor(i => i.BookId).NotEmpty();
        }
        public bool ValidateTCNumber(string tcNumber)
        {
            if (string.IsNullOrEmpty(tcNumber)) return false;
            if (tcNumber.Length != 11) return false;

            int c1 = 0, c2 = 0;
            for (int i = 0; i < 9; i++)
            {
                int n = tcNumber[i] - '0';
                if (n < 0 || n > 9) return false;
                c1 += n;
                c2 += c1;
            }
            int n10 = tcNumber[9] - '0', n11 = tcNumber[10] - '0';
            if (n10 != ((c2 % 10) + n11) % 10) return false;
            if ((c1 + c2 + n10 + n11) % 10 != 0) return false;

            return true;
        }
    }
}
