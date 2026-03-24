using blaLink.Data;
using blaLink.Models;
using blaLink.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;


namespace YourProjectName.Controllers
{
    public class LinksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShortenerService _shortenerService;

        // This is the constructor: where you request the system to provide the Database and the shortener Service
        public LinksController(ApplicationDbContext context, ShortenerService shortenerService)
        {
            _context = context;
            _shortenerService = shortenerService;
        }

        // 1. Action that displays the link input page (UI)
        // When you visit the path: /Links/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 2. Action that handles when the user clicks the "Shorten" button (save data)
        [HttpPost]
        public async Task<IActionResult> Create(string originalUrl, string? customAlias)
        {
            if (string.IsNullOrEmpty(originalUrl))
            {
                return View(); // If no link entered, prompt to re-enter
            }

            var shortLink = new ShortLink
            {
                OriginalUrl = originalUrl,
                CreatedDate = DateTime.Now
            };

            // Case 1: User wants a "nice name" (Custom Alias)
            if (!string.IsNullOrEmpty(customAlias))
            {
                // Check whether this name is already used
                var exists = await _context.ShortLinks.AnyAsync(s => s.ShortCode == customAlias);
                if (exists)
                {
                    ViewBag.Error = "This name is already taken!";
                    return View();
                }
                shortLink.ShortCode = customAlias;
            }

            // Save first time to DB to get auto-increment ID (if no Custom Alias)
            _context.ShortLinks.Add(shortLink);
            await _context.SaveChangesAsync();

            // Case 2: If not using custom alias, now use the ID to create the ShortCode
            if (string.IsNullOrEmpty(customAlias))
            {
                shortLink.ShortCode = _shortenerService.Encode(shortLink.Id);
                await _context.SaveChangesAsync(); // Update ShortCode back to DB
            }

            // 1. Create QR code (originalUrl)
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(originalUrl, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);

            // 2. Convert byte array to Base64 string to display on HTML
            string qrCodeBase64 = Convert.ToBase64String(qrCodeAsPngByteArr);

            // 3. Send this string to the View via ViewBag
            ViewBag.QrCodeAsBase64 = qrCodeBase64;
            // After finishing, send data to the "Result" view to show the short link
            return View("Result", shortLink);
        }
    }
}