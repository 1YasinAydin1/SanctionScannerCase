namespace SanctionScannerCase.Application.Contracts.Books
{
    public class BooksCreateDto
    {
        public string? Name { get; set; }
        public int? PageCount { get; set; }
        public byte[]? Image { get; set; }
        public string? Author { get; set; }
        public string? Type { get; set; }
        public string? CoverLetter { get; set; }
        public bool Status { get; set; } = true;
    }
}
