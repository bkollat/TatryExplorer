using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TatryExplorer.Models;

namespace TatryExplorer.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Trail> Trails { get; set; }
        public DbSet<FavoriteTrail> FavoriteTrails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FavoriteTrail>()
                .HasOne(ft => ft.Trail)
                .WithMany(t => t.FavoriteTrails)
                .HasForeignKey(ft => ft.TrailId);

            builder.Entity<FavoriteTrail>()
                .HasOne(ft => ft.User)
                .WithMany()
                .HasForeignKey(ft => ft.UserId);
        }
    }
}