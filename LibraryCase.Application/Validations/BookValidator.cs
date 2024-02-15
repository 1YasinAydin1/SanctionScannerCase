using FluentValidation;
using LibraryCase.Application.Contracts.Books;

namespace LibraryCase.Application.Validations
{
    public class BookValidator : AbstractValidator<BooksCreateDto>
    {
        public BookValidator()
        {
            // Lokalizasyon sağlanabilir ileride
            RuleFor(i => i.Name).NotEmpty().NotNull();
            RuleFor(i => i.Author).NotEmpty();
            RuleFor(i => i.Type).NotEmpty();
            RuleFor(i => i.PageCount).NotEmpty().GreaterThanOrEqualTo(1);
            RuleFor(i => i.CoverLetter).NotEmpty();
            RuleFor(i => i.Image).NotEmpty();
        }
    }
}
