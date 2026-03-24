using blaLink.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace blaLink.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortLink> ShortLinks { get; set; }
    }
}