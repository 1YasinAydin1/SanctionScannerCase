using AutoMapper;
using SanctionScannerCase.Application.Contracts.Books;
using SanctionScannerCase.Application.Contracts.BorrowedBook;
using SanctionScannerCase.Domain.Books;
using SanctionScannerCase.Domain.BorrowedBooks;

namespace SanctionScannerCase
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<BooksCreateDto, Book>();
            CreateMap<BorrowedBookCreateDto, BorrowedBook>();
            CreateMap<BorrowedBookGetDto, BorrowedBook>();
        }
    }
}
