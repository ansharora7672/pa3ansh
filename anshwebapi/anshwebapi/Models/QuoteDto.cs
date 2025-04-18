namespace anshwebapi.Models
{
    public class QuoteDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string? Author { get; set; }
        public int Likes { get; set; }
    }
}
