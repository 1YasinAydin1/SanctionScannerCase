

using Microsoft.EntityFrameworkCore;
using SanctionScannerCase.Domain.Books;
using SanctionScannerCase.Domain.BorrowedBooks;
using SanctionScannerCase.Domain.Common;

namespace SanctionScannerCase.EntityFrameworkCore.Context
{
    public class SanctionScannerDbContext : DbContext
    {
        public SanctionScannerDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=SanctionScannerCase;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BorrowedBook>()
                  .HasOne(b => b.Book)
                  .WithMany(a => a.BorrowedBooks)
                  .HasForeignKey(b => b.BookId);
        }
        /// <summary>
        /// İşlemler Database üzerine aktarılırken araya girerek BaseEntity alanı dolduruldu.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<BaseEntity>())
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        item.Entity.CreateDate = DateTime.UtcNow;
                        break;
                    default:
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
