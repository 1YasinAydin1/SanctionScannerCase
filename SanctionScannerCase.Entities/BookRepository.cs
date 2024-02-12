using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SanctionScannerCase.Domain.Books;
using SanctionScannerCase.Domain.BorrowedBooks;
using SanctionScannerCase.EntityFrameworkCore.Context;

namespace SanctionScannerCase.EntityFrameworkCore
{
    public class BookRepository : IBookRepository
    {
        private readonly SanctionScannerDbContext dbContext;
        private readonly ILogger<BookRepository> logger;

        public BookRepository(SanctionScannerDbContext dbContext, ILogger<BookRepository> _logger)
        {
            this.dbContext = dbContext;
            logger = _logger;
        }
        public DbSet<Book> Table => dbContext.Set<Book>();
        public DbSet<BorrowedBook> BorrowedBookTable => dbContext.Set<BorrowedBook>();
        public async Task<bool> CreateBookAysnc(Book book)
        {
            EntityEntry<Book> entityEntry = await Table.AddAsync(book);
            if (entityEntry.State == EntityState.Added)
            {
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                logger.LogError($"Book Entity state is not Added. Model -> {JsonConvert.SerializeObject(book)}");
                return false;
            }
        }
        public async Task<bool> UpdateBookForBorrowedAysnc(Guid Id, bool status)
        {
            var book = await Table.FirstOrDefaultAsync(i => i.Id.Equals(Id));
            if (book != null)
            {
                book.Status = status;
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                logger.LogError($"Book Entity state is not Found. Id = {Id}");
                return false;
            }
        }
        public async Task<Book> GetBookByIdAsync(Guid Id)
        {
            var response = await Table.Where(i => i.Id.Equals(Id)).FirstOrDefaultAsync();
            if (response != null)
                return response;
            logger.LogError($"Book Entity state is not Found. Id = {Id}");
            return null;
        }
        public IQueryable<BookJoinBorrowedBook> GetBookNavigationAll()
        {
            return (from book in Table
                    join borrowedBook in BorrowedBookTable on book.Id equals borrowedBook.BookId into gj
                    from subBorrowedBook in gj.DefaultIfEmpty()
                    select new BookJoinBorrowedBook
                    {
                        Id = book.Id,
                        Name = book.Name,
                        PageCount = book.PageCount,
                        Image = book.Image,
                        Author = book.Author,
                        Status = book.Status,
                        Type = book.Type,
                        CoverLetter = book.CoverLetter,
                        BorrowedBooks = subBorrowedBook
                    });
        }
    }
}
