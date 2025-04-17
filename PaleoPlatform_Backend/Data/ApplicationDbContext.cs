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
                .HasForeignKey(a => a.AutoreId)
                .OnDelete(DeleteBehavior.NoAction); // just to be safe

            builder.Entity<Commento>()
                .HasOne(c => c.ParentComment)
                .WithMany()
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.ClientSetNull); // avoid cascade delete on replies

            builder.Entity<Commento>()
                .HasOne(c => c.Utente)
                .WithMany()
                .HasForeignKey(c => c.UtenteId)
                .OnDelete(DeleteBehavior.NoAction); // avoid cascade delete on user deletion
        }

        // Add this line to expose the Files table
        public DbSet<UploadedFile> Files { get; set; }

        // DbSets for your other entities
        public DbSet<Articolo> Articoli { get; set; }
        public DbSet<Commento> Commenti { get; set; }
    }
}