using Microsoft.EntityFrameworkCore;
using CoworkerHub.Core;

namespace CoworkerHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : 
            base(options)
        {

        }
        
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Desk> Desks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Workspace>()
                .HasMany(w => w.Desks)
                .WithOne(d => d.Workspace)
                .HasForeignKey(d => d.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
