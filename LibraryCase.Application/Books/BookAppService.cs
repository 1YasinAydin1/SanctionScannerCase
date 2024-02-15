using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LibraryCase.Application.Contracts.Books;
using LibraryCase.Application.Contracts.Results;
using LibraryCase.Domain.Books;
using LibraryCase.Domain.BorrowedBooks;

namespace LibraryCase.Application.Books
{
    public class BookAppService : IBookAppService
    {
        private readonly IBookRepository bookRepository;
        private readonly IBorrowedBookRepository borrowedBookRepository;
        private readonly IMapper mapper;
        private readonly ILogger<BookAppService> logger;
        public BookAppService(IBookRepository bookRepository, IBorrowedBookRepository borrowedBookRepository, IMapper mapper, ILogger<BookAppService> _logger)
        {
            this.bookRepository = bookRepository;
            this.borrowedBookRepository = borrowedBookRepository;
            this.mapper = mapper;
            logger = _logger;
        }

        public async Task<BookDto> GetBookById(Guid Id)
        {
            logger.LogInformation($"BookAppService -> GetBookById | Request | Guid Id -> {Id}");
            var response = await bookRepository.GetBookByIdAsync(Id);
            return mapper.Map<BookDto>(response);
        }
        public Response<List<BookJoinBorrowedBookDto>> GetBooksAsync()
        {
            logger.LogInformation($"BookAppService -> GetBooksAsync | Request ");
            IQueryable<BookJoinBorrowedBook> result = bookRepository.GetBookNavigationAll();
            result = result.GroupBy(item => item.Id)
                                    .Select(group =>
                                    group.OrderByDescending(item =>
                                    item.BorrowedBooks != null ? item.BorrowedBooks.CreateDate : DateTime.MinValue).First());
           return Response<List<BookJoinBorrowedBookDto>>.Success(JsonConvert.DeserializeObject<List<BookJoinBorrowedBookDto>>(JsonConvert.SerializeObject(result)));
        }

        public async Task<Response<string>> PostBooksAsync(BooksCreateDto postRequest)
        {
            bool result = await bookRepository.CreateBookAysnc(mapper.Map<Book>(postRequest));
            if (!result)
                logger.LogError($"BookAppService -> PostBooksAsync -> CreateBookAysnc | The server encountered an unexpected error");
            return result ? Response<string>.Success("Başarılı") : Response<string>.Fail(string.Empty);
        }
    }
}
