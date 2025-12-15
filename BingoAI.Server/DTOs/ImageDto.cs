namespace BingoAI.Server.DTOs
{
    /// <summary>
    /// DTO pour la réponse d'une image (sans les données binaires).
    /// </summary>
    public class ImageMetadataDto
    {
        public required Guid Id { get; init; }
        public required string FileName { get; init; }
        public required string ContentType { get; init; }
        public required long FileSize { get; init; }
        public required DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public string? Description { get; init; }
        public string? Tags { get; init; }
    }

    /// <summary>
    /// DTO pour la création/mise à jour des métadonnées d'une image
    /// </summary>
    public class ImageUpdateDto
    {
        public string? Description { get; init; }
        public string? Tags { get; init; }
    }

    /// <summary>
    /// DTO pour l'upload d'une image avec ses métadonnées
    /// </summary>
    public class ImageUploadDto
    {
        public required IFormFile File { get; init; }
        public string? Description { get; init; }
        public string? Tags { get; init; }
    }
}
