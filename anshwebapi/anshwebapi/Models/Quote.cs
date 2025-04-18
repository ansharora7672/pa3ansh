using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace anshwebapi.Models
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = null!; 

        public string? Author { get; set; }

        public int Likes { get; set; } = 0;

        public ICollection<QuoteTag> QuoteTags { get; set; } = new List<QuoteTag>();
    }
}
