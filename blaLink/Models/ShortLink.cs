using System.ComponentModel.DataAnnotations;

namespace blaLink.Models
{
    public class ShortLink
    {
        // Data annotations to define the database schema and validation rules
        // [Key] helps EF Core know this is the primary key, auto-incrementing (1, 2, 3...)
        [Key]
        public int Id { get; set; }

        // [Required] means the original link cannot be empty
        // [Url] requires the user to enter a valid http://... format
        [Required]
        [Url]
        public string OriginalUrl { get; set; } = string.Empty;

        // Short code (e.g., aB3x9). We'll limit the length for brevity.
        [StringLength(20)]
        public string ShortCode { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int ClickCount { get; set; } = 0;
    }
}
