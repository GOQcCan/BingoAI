namespace BingoAI.Server.Models
{
    /// <summary>
    /// Entité représentant une image stockée avec ses métadonnées.
    /// </summary>
    public class ImageEntity
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// Nom original du fichier uploadé
        /// </summary>
        public required string FileName
        {
            get;
            set => field = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Le nom de fichier ne peut pas être vide.", nameof(FileName)) : value;
        }
        
        /// <summary>
        /// Type MIME de l'image (ex: image/png, image/jpeg)
        /// </summary>
        public required string ContentType
        {
            get;
            set => field = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Le type de contenu ne peut pas être vide.", nameof(ContentType)) : value;
        }
        
        /// <summary>
        /// Données binaires de l'image
        /// </summary>
        public required byte[] Data
        {
            get;
            set => field = value.Length == 0 ? throw new ArgumentException("Les données ne peuvent pas être vides.", nameof(Data)) : value;
        }
        
        /// <summary>
        /// Taille du fichier en bytes
        /// </summary>
        public long FileSize
        {
            get;
            set => field = value <= 0 ? throw new ArgumentException("La taille du fichier doit être positive.", nameof(FileSize)) : value;
        }
        
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
