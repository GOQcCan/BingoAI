using BingoAI.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace BingoAI.Server.Data
{
    /// <summary>
    /// Contexte de base de données Entity Framework Core.
    /// Suit le principe d'inversion des dépendances (DIP) via DbContextOptions.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Table des images
        /// </summary>
        public DbSet<ImageEntity> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ImageEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.ContentType)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Data)
                    .IsRequired();
                
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.Tags)
                    .HasMaxLength(500);
                
                entity.Property(e => e.UserId)
                    .HasMaxLength(255);

                // Index pour optimiser les recherches par utilisateur
                entity.HasIndex(e => e.UserId);
                
                // Index pour optimiser les recherches par date
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}
