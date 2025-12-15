using BingoAI.Server.Models;

namespace BingoAI.Server.Extensions;
/// <summary>
/// Extension members pour ImageEntity
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
        /// Vérifie si l'image appartient à un utilisateur spécifique
        /// </summary>
        public bool BelongsTo(string userId) => 
            !string.IsNullOrEmpty(image.UserId) && image.UserId == userId;

        /// <summary>
        /// Vérifie si la taille du fichier est valide
        /// </summary>
        public bool IsValidSize => image.FileSize > 0 && image.FileSize <= MaxFileSizeBytes;

        /// <summary>
        /// Vérifie si le type de contenu de l'image est valide
        /// </summary>
        public bool IsValidContentType => ImageEntity.IsSupportedContentType(image.ContentType);

        /// <summary>
        /// Vérifie si l'image est valide (taille et type de contenu)
        /// </summary>
        public bool IsValid => image.IsValidSize && image.IsValidContentType;
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