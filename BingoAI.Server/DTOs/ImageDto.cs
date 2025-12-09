namespace BingoAI.Server.DTOs
{
    /// <summary>
    /// DTO pour la réponse d'une image (sans les données binaires).
    /// Suit le principe de ségrégation des interfaces (ISP) - expose uniquement les métadonnées.
    /// </summary>
    public class ImageMetadataDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
    }

    /// <summary>
    /// DTO pour la création/mise à jour des métadonnées d'une image
    /// </summary>
    public class ImageUpdateDto
    {
        public string? Description { get; set; }
        public string? Tags { get; set; }
    }

    /// <summary>
    /// DTO pour l'upload d'une image avec ses métadonnées
    /// </summary>
    public class ImageUploadDto
    {
        public IFormFile File { get; set; } = null!;
        public string? Description { get; set; }
        public string? Tags { get; set; }
    }
}
