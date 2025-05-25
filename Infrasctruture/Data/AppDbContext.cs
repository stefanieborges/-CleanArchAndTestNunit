using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrasctruture.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
        { }

        public DbSet<ApplicationUser> Usuarios { get; set; }
    }
}
