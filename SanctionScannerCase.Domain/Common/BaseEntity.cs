namespace LibraryCase.Domain.Common
{
    /// <summary>
    /// Tüm Entity classlarda ortak alanlar BaseEntity içine alınmıştır
    /// </summary>
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
