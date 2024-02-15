namespace LibraryCase.Application.Contracts.Books
{
    public class BookDto
    {
        public Guid Id{ get; set; }
        public string Name { get; set; }
        public int PageCount { get; set; }
        public byte[] Image { get; set; }
        public string Author { get; set; }
        public bool Status { get; set; }
        public string Type { get; set; }
        public string CoverLetter { get; set; }
    }
}
