using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LibraryCase.Domain.BorrowedBooks;
using LibraryCase.EntityFrameworkCore.Context;

namespace LibraryCase.EntityFrameworkCore
{
    public class BorrowedBookRepository : IBorrowedBookRepository
    {
        private readonly SanctionScannerDbContext dbContext;
        private readonly ILogger<BorrowedBookRepository> logger;

        public BorrowedBookRepository(SanctionScannerDbContext dbContext, ILogger<BorrowedBookRepository> _logger)
        {
            this.dbContext = dbContext;
            logger = _logger;
        }
        public DbSet<BorrowedBook> Table => dbContext.Set<BorrowedBook>();
        public async Task<bool> CreateBorrowedBookAysnc(BorrowedBook borrowedBook)
        {
            EntityEntry<BorrowedBook> entityEntry = await Table.AddAsync(borrowedBook);
            if (entityEntry.State == EntityState.Added)
            {
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                logger.LogError($"BorrowedBook Entity state is not Added. Model -> {JsonConvert.SerializeObject(borrowedBook)}");
                return false;
            }
        }

        public async Task<List<BorrowedBook>> GetBorrowedBookAllAsync()
        {
            return await Table.OrderBy(i=>i.BorrowerName).ToListAsync();
        }

        public async Task<bool> UpdateBorrowedBokForReturnAysnc(Guid Id)
        {
            var borrowedBook = await Table.FirstOrDefaultAsync(i => i.Id.Equals(Id));
            if (borrowedBook != null)
            {
                borrowedBook.ActualReturnDate = DateTime.Now.Date;
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                logger.LogError($"BorrowedBook Entity state is not Found. Id -> {Id}");
                return false;
            }
        }
    }
}
