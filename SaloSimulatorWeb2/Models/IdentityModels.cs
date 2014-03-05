using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Salo.SaloSimulatorWeb2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Bot> Bots { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
    }
}