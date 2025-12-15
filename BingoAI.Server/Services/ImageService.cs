using BingoAI.Server.Data;
using BingoAI.Server.Extensions;
using BingoAI.Server.Models;
using BingoAI.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BingoAI.Server.Services
{
    /// <summary>
    /// Service de gestion des images.
    /// </summary>
    public class ImageService(AppDbContext context, ILogger<ImageService> logger) : IImageService
    {
        // Les constantes locales ont été supprimées au profit des extensions statiques sur ImageEntity

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

            // Utilisation de la propriété d'extension IsValid pour garantir l'intégrité de l'entité avant la sauvegarde
            if (!image.IsValid)
            {
                throw new InvalidOperationException("L'entité image générée ne respecte pas les règles de validation (taille ou type de contenu).");
            }

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

            // Utilisation des extensions statiques C# 14 sur le type ImageEntity
            if (file.Length > ImageEntity.MaxFileSize)
            {
                throw new ArgumentException($"Le fichier dépasse la taille maximale autorisée ({ImageEntity.MaxFileSize / (1024 * 1024)} MB).", nameof(file));
            }

            if (!ImageEntity.IsSupportedContentType(file.ContentType))
            {
                throw new ArgumentException($"Type de fichier non autorisé: {file.ContentType}. Types autorisés: {string.Join(", ", ImageEntity.GetSupportedContentTypes())}", nameof(file));
            }
        }
    }
}
