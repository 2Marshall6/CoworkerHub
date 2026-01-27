using Microsoft.EntityFrameworkCore;
using CoworkerHub.Core;

namespace CoworkerHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : 
            base(options)
        {

        }
        
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Desk> Desks { get; set; }
    }
}
