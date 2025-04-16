using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PaleoPlatform_Backend.Models;

namespace PaleoPlatform_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Articolo>()
                .HasOne(a => a.Autore)
                .WithMany()
                .HasForeignKey(a => a.AutoreId);
        }

        // Add this line to expose the Files table
        public DbSet<UploadedFile> Files { get; set; }

        // DbSets for your other entities
        public DbSet<Articolo> Articoli { get; set; }
        // public DbSet<Commento> Commenti { get; set; }
    }
}