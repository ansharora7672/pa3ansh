using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using anshwebapi.Data;
using anshwebapi.Models;

namespace anshwebapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public QuotesController(AppDbContext db) => _db = db;

        // Create a new quote
        // POST /api/quotes
        [HttpPost]
        public async Task<ActionResult<Quote>> Create([FromBody] Quote quote)
        {
            if (string.IsNullOrWhiteSpace(quote.Content))
                return BadRequest("Content is required.");
            _db.Quotes.Add(quote);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = quote.Id }, quote);
        }

        // Get a single quote by its ID
        // GET /api/quotes/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Quote>> GetById(int id)
        {
            var quote = await _db.Quotes.FindAsync(id);
            if (quote == null) return NotFound();
            return Ok(quote);
        }

        // Update an existing quote's content and author
        // PUT /api/quotes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Quote updated)
        {
            if (id != updated.Id) return BadRequest("ID mismatch.");
            var existing = await _db.Quotes.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Content = updated.Content;
            existing.Author = updated.Author;
            await _db.SaveChangesAsync();
            return NoContent();
        }


        // Delete a quote by its ID
        // DELETE /api/quotes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var quote = await _db.Quotes.FindAsync(id);
            if (quote == null) return NotFound();
            _db.Quotes.Remove(quote);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Increment the like count for a quote
        // POST /api/quotes/{id}/like
        [HttpPost("{id:int}/like")]
        public async Task<IActionResult> Like(int id)
        {
            var quote = await _db.Quotes.FindAsync(id);
            if (quote == null) return NotFound();
            quote.Likes++;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Get the top liked quotes
        // GET /api/quotes/top
        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<Quote>>> GetTop([FromQuery] int count = 10)
        {
            var topQuotes = await _db.Quotes
                .OrderByDescending(q => q.Likes)
                .Take(count)
                .ToListAsync();
            return Ok(topQuotes);
        }

 
        // POST /api/quotes/{id}/tags
        [HttpPost("{id:int}/tags")]
        public async Task<IActionResult> AddTag(int id, [FromBody] Tag tag)
        {
     
            var quote = await _db.Quotes
                .Include(q => q.QuoteTags)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (quote == null)
                return NotFound();

         
            var existingTag = await _db.Tags
                .FirstOrDefaultAsync(t => t.Name == tag.Name);

            if (existingTag != null)
            {
            
                tag = existingTag;
            }
            else
            {
     
                _db.Tags.Add(tag);
                await _db.SaveChangesAsync();
            }

          
            if (!quote.QuoteTags.Any(qt => qt.TagId == tag.Id))
            {
                quote.QuoteTags.Add(new QuoteTag
                {
                    QuoteId = quote.Id,
                    TagId = tag.Id
                });
                await _db.SaveChangesAsync();
            }

            return NoContent();
        }

        // List all tags for a specific quote
        // GET /api/quotes/{id}/tags
        [HttpGet("{id:int}/tags")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags(int id)
        {
            var quote = await _db.Quotes
                .Include(q => q.QuoteTags)
                    .ThenInclude(qt => qt.Tag)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
                return NotFound();

            var tagDtos = quote.QuoteTags
                .Select(qt => new TagDto
                {
                    Id = qt.Tag.Id,
                    Name = qt.Tag.Name
                })
                .ToList();

            return Ok(tagDtos);
        }

        // Get all quotes
        // GET /api/quotes
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<QuoteDto>>> GetAll()
        {
            var quotes = await _db.Quotes
                .Select(q => new QuoteDto
                {
                    Id = q.Id,
                    Content = q.Content,
                    Author = q.Author,
                    Likes = q.Likes
                })
                .ToListAsync();

            return Ok(quotes);
        }

        // Get all quotes for a specific tag
        // GET /api/quotes/tags/{tagId}
        [HttpGet("tags/{tagId:int}")]
        public async Task<ActionResult<IEnumerable<QuoteDto>>> GetByTag(int tagId)
        {
   
            var tag = await _db.Tags
                .Include(t => t.QuoteTags)
                    .ThenInclude(qt => qt.Quote)
                .FirstOrDefaultAsync(t => t.Id == tagId);

            if (tag == null)
                return NotFound($"Tag {tagId} not found.");

  
            var quotes = tag.QuoteTags
                .Select(qt => new QuoteDto
                {
                    Id = qt.Quote.Id,
                    Content = qt.Quote.Content,
                    Author = qt.Quote.Author,
                    Likes = qt.Quote.Likes
                })
                .ToList();

            return Ok(quotes);
        }


        // List all current tags
        // GET /api/tags
        [HttpGet("/api/tags")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTags()
        {
            var tags = await _db.Tags
                .Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();

            return Ok(tags);
        }



    }
}
