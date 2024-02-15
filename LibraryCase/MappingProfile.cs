using AutoMapper;
using LibraryCase.Application.Contracts.Books;
using LibraryCase.Application.Contracts.BorrowedBook;
using LibraryCase.Domain.Books;
using LibraryCase.Domain.BorrowedBooks;

namespace LibraryCase
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
