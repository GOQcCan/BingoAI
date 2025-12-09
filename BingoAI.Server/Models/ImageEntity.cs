namespace BingoAI.Server.Models
{
    /// <summary>
    /// Entité représentant une image stockée avec ses métadonnées.
    /// Suit le principe de responsabilité unique (SRP) - ne contient que les données de l'image.
    /// </summary>
    public class ImageEntity
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// Nom original du fichier uploadé
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        
        /// <summary>
        /// Type MIME de l'image (ex: image/png, image/jpeg)
        /// </summary>
        public string ContentType { get; set; } = string.Empty;
        
        /// <summary>
        /// Données binaires de l'image
        /// </summary>
        public byte[] Data { get; set; } = [];
        
        /// <summary>
        /// Taille du fichier en bytes
        /// </summary>
        public long FileSize { get; set; }
        
        /// <summary>
        /// Date de création de l'enregistrement
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Date de dernière modification
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Description optionnelle de l'image
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Tags ou mots-clés associés à l'image (format CSV ou JSON)
        /// </summary>
        public string? Tags { get; set; }
        
        /// <summary>
        /// Identifiant de l'utilisateur propriétaire de l'image
        /// </summary>
        public string? UserId { get; set; }
    }
}
