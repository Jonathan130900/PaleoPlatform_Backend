﻿using System.Reflection.Emit;
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

            builder.Entity<ExpiredToken>()
                .HasIndex(t => t.Token)
                .IsUnique(false); // Allow duplicate tokens

            builder.Entity<Articolo>()
                .HasOne(a => a.Autore)
                .WithMany()
                .HasForeignKey(a => a.AutoreId)
                .OnDelete(DeleteBehavior.NoAction); // just to be safe

            builder.Entity<Commento>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.ClientSetNull); // avoid cascade delete on replies

            builder.Entity<Commento>()
                .HasOne(c => c.Utente)
                .WithMany()
                .HasForeignKey(c => c.UtenteId)
                .OnDelete(DeleteBehavior.NoAction); // avoid cascade delete on user deletion

            builder.Entity<Commento>()
                .HasOne(c => c.Discussione)
                .WithMany(d => d.Commenti)
                .HasForeignKey(c => c.DiscussioneId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Commento>()
                .HasOne(c => c.Articolo)
                .WithMany(a => a.Commenti)
                .HasForeignKey(c => c.ArticoloId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Biglietto>()
                .HasOne(b => b.Evento)
                .WithMany(e => e.Biglietti)
                .HasForeignKey(b => b.EventoId);

            builder.Entity<Biglietto>()
                .HasOne(b => b.Utente)
                .WithMany(u => u.Biglietti)
                .HasForeignKey(b => b.UtenteId);

            builder.Entity<Biglietto>()
                .Property(b => b.Prezzo)
                .HasPrecision(18, 2);

            builder.Entity<Evento>()
                .Property(e => e.Prezzo)
                .HasPrecision(18, 2);

            builder.Entity<EventoPartecipazione>()
                .HasIndex(p => new { p.UtenteId, p.EventoId })
                .IsUnique();

            builder.Entity<Prodotto>()
                .Property(p => p.Prezzo)
                .HasPrecision(10, 2);
        }

        // DbSets for entities
        public DbSet<UploadedFile> Files { get; set; }
        public DbSet<Articolo> Articoli { get; set; }
        public DbSet<Commento> Commenti { get; set; }
        public DbSet<Discussione> Discussione { get; set; }
        public DbSet<Topics> Topics { get; set; }
        public DbSet<Evento> Eventi { get; set; }
        public DbSet<Biglietto> Biglietti { get; set; }
        public DbSet<EventoPartecipazione> EventoPartecipazioni { get; set; }
        public DbSet<Prodotto> Prodotti { get; set; }
        public DbSet<Carrello> Carrelli { get; set; }
        public DbSet<CarrelloItem> CarrelloItems { get; set; }
        public DbSet<ExpiredToken> ExpiredTokens { get; set; }

    }
}
