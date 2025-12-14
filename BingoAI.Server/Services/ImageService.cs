using BingoAI.Server.Data;
using BingoAI.Server.Models;
using BingoAI.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BingoAI.Server.Services
{
    /// <summary>
    /// Service de gestion des images.
    /// Suit le principe de responsabilité unique (SRP) - gère uniquement la logique métier des images.
    /// Suit le principe ouvert/fermé (OCP) - peut être étendu sans modifier le code existant.
    /// </summary>
    public class ImageService(AppDbContext context, ILogger<ImageService> logger) : IImageService
    {
        // Types MIME autorisés pour les images
        private static readonly HashSet<string> AllowedContentTypes =
        [
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp",
            "image/bmp",
            "image/svg+xml"
        ];

        // Taille maximale de fichier (10 MB)
        private const long MaxFileSize = 10 * 1024 * 1024;

        public async Task<ImageEntity> SaveImageAsync(IFormFile file, string? userId, string? description = null, string? tags = null)
        {
            // Validation du fichier
            ValidateFile(file);

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var image = new ImageEntity
            {
                FileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                Data = memoryStream.ToArray(),
                FileSize = file.Length,
                Description = description,
                Tags = tags,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.Images.Add(image);
            await context.SaveChangesAsync();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Image saved: {FileName} (ID: {Id}) for user: {UserId}",
                    image.FileName, image.Id, userId);
            }

            return image;
        }

        public async Task<ImageEntity?> GetImageByIdAsync(Guid id)
        {
            return await context.Images.FindAsync(id);
        }

        public async Task<IEnumerable<ImageEntity>> GetImagesByUserIdAsync(string userId)
        {
            return await context.Images
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<ImageEntity?> UpdateImageMetadataAsync(Guid id, string? description, string? tags)
        {
            var image = await context.Images.FindAsync(id);
            
            if (image is null)
            {
                return null;
            }

            // Utilisation du null-conditional assignment de C# 14
            image?.Description = description;
            image?.Tags = tags;
            image?.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Image metadata updated: ID {Id}", id);
            }

            return image;
        }

        public async Task<bool> DeleteImageAsync(Guid id)
        {
            var image = await context.Images.FindAsync(id);
            
            if (image is null)
            {
                return false;
            }

            context.Images.Remove(image);
            await context.SaveChangesAsync();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Image deleted: ID {Id}", id);
            }

            return true;
        }

        private static void ValidateFile(IFormFile file)
        {
            ArgumentNullException.ThrowIfNull(file, nameof(file));

            if (file.Length == 0)
            {
                throw new ArgumentException("Le fichier est vide.", nameof(file));
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException($"Le fichier dépasse la taille maximale autorisée ({MaxFileSize / (1024 * 1024)} MB).", nameof(file));
            }

            if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                throw new ArgumentException($"Type de fichier non autorisé: {file.ContentType}. Types autorisés: {string.Join(", ", AllowedContentTypes)}", nameof(file));
            }
        }
    }
}
