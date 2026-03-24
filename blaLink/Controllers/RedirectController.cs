using blaLink.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YourProjectName.Controllers
{
    public class RedirectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RedirectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // [Route("/{code}")] means it will catch everything after '/'
        // For example: localhost:xxxx/abc => "abc" will be assigned to the code variable
        [HttpGet("/{code}")]
        public async Task<IActionResult> Index(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index", "Home");
            }

            // 1. Find in the database which original link this "code" points to
            var link = await _context.ShortLinks
                                     .FirstOrDefaultAsync(x => x.ShortCode == code);

            // 2. If the code is not found in the DB
            if (link == null)
            {
                return NotFound(); // Return 404 page
            }

            // 3. Increment click count (for analytics/statistics)
            link.ClickCount++;
            await _context.SaveChangesAsync();

            // 4. Redirect the user to the original URL
            return Redirect(link.OriginalUrl);
        }
    }
}