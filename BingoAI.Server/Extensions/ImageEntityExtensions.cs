using BingoAI.Server.Models;

namespace BingoAI.Server.Extensions;
/// <summary>
/// Extension members pour ImageEntity utilisant les nouvelles fonctionnalités de C# 14
/// </summary>
public static class ImageEntityExtensions
{
    // Constantes statiques en dehors de l'extension block
    private static readonly IReadOnlySet<string> _supportedContentTypes = new HashSet<string>
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "image/bmp",
        "image/svg+xml"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    // Extension block pour les méthodes d'instance
    extension(ImageEntity image)
    {
        /// <summary>
        /// Vérifie si l'image a été modifiée récemment (dans les dernières 24 heures)
        /// </summary>
        public bool IsRecentlyModified => 
            image.UpdatedAt.HasValue && (DateTime.UtcNow - image.UpdatedAt.Value).TotalHours < 24;

        /// <summary>
        /// Obtient la taille du fichier en format lisible (KB, MB)
        /// </summary>
        public string FormattedFileSize
        {
            get
            {
                const long KB = 1024;
                const long MB = KB * 1024;

                return image.FileSize switch
                {
                    < KB => $"{image.FileSize} B",
                    < MB => $"{image.FileSize / (double)KB:F2} KB",
                    _ => $"{image.FileSize / (double)MB:F2} MB"
                };
            }
        }

        /// <summary>
        /// Obtient l'extension du fichier
        /// </summary>
        public string FileExtension => Path.GetExtension(image.FileName).TrimStart('.');

        /// <summary>
        /// Vérifie si l'image appartient à un utilisateur spécifique
        /// </summary>
        public bool BelongsTo(string userId) => 
            !string.IsNullOrEmpty(image.UserId) && image.UserId == userId;

        /// <summary>
        /// Vérifie si la taille du fichier est valide
        /// </summary>
        public bool IsValidSize => image.FileSize > 0 && image.FileSize <= MaxFileSizeBytes;
    }

    // Extension block pour les méthodes statiques
    extension(ImageEntity)
    {
        /// <summary>
        /// Taille maximale de fichier en bytes
        /// </summary>
        public static long MaxFileSize => MaxFileSizeBytes;

        /// <summary>
        /// Vérifie si un type de contenu est supporté
        /// </summary>
        public static bool IsSupportedContentType(string contentType) =>
            _supportedContentTypes.Contains(contentType?.ToLowerInvariant() ?? string.Empty);
        
        /// <summary>
        /// Obtient la liste des types de contenu supportés
        /// </summary>
        public static IReadOnlySet<string> GetSupportedContentTypes() => _supportedContentTypes;
    }
}