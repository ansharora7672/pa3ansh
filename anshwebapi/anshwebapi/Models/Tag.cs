using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace anshwebapi.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        public ICollection<QuoteTag> QuoteTags { get; set; } = new List<QuoteTag>();
    }
}
