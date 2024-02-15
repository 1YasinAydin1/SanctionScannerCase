using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LibraryCase.Application.Contracts.BorrowedBook;
using LibraryCase.Application.Contracts.Results;
using LibraryCase.Domain.Books;
using LibraryCase.Domain.BorrowedBooks;

namespace LibraryCase.Application.BorrowedBooks
{
    public class BorrowedBookAppService : IBorrowedBookAppService
    {
        private readonly IBorrowedBookRepository borrowedBookRepository;
        private readonly IBookRepository bookRepository;
        private readonly IMapper mapper;
        private readonly ILogger<BorrowedBookAppService> logger;

        public BorrowedBookAppService(IBorrowedBookRepository borrowedBookRepository, IBookRepository bookRepository, IMapper mapper, ILogger<BorrowedBookAppService> logger)
        {
            this.borrowedBookRepository = borrowedBookRepository;
            this.bookRepository = bookRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Response<string>> PostBorrowedBooksAsync(BorrowedBookCreateDto postRequest)
        {
            logger.LogInformation($"BorrowedBookAppService -> PostBorrowedBooksAsync | Request | BorrowedBookCreateDto  -> {JsonConvert.SerializeObject(postRequest)}");
            bool result = await borrowedBookRepository.CreateBorrowedBookAysnc(mapper.Map<BorrowedBook>(postRequest));
            if (result)
                await bookRepository.UpdateBookForBorrowedAysnc(postRequest.BookId, false);
            else
                logger.LogError($"BorrowedBookAppService -> PostReturnBooksAsync -> CreateBorrowedBookAysnc | The server encountered an unexpected error");
            return result ? Response<string>.Success("Başarılı") : Response<string>.Fail(string.Empty);
        }

        public async Task<Response<string>> PostReturnBooksAsync(Guid Id, Guid BookId)
        {
            logger.LogInformation($"BorrowedBookAppService -> PostReturnBooksAsync | Request | Guid Id -> {Id},Guid BookId  > {BookId}");
            bool result = await borrowedBookRepository.UpdateBorrowedBokForReturnAysnc(Id);
            if (result)
                await bookRepository.UpdateBookForBorrowedAysnc(BookId, true);
            else
                logger.LogError($"BorrowedBookAppService -> PostReturnBooksAsync -> UpdateBorrowedBokForReturnAysnc | The server encountered an unexpected error");
            return result ? Response<string>.Success("Başarılı") : Response<string>.Fail(string.Empty);
        }
    }
}
