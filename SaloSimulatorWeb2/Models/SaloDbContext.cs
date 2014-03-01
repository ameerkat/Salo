using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SaloSimulatorWeb2.Models
{
    public class SaloDbContext : DbContext
    {
        public DbSet<BotModel> Bots { get; set; }
    }
}