using BingoAI.Server.Models;
using Microsoft.AspNetCore.Http;

namespace BingoAI.Server.Services.Interfaces
{
    /// <summary>
    /// Interface pour le service de gestion des images.
    /// Suit le principe d'inversion des dépendances (DIP) et de ségrégation des interfaces (ISP).
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Sauvegarde une image uploadée
        /// </summary>
        /// <param name="file">Fichier uploadé</param>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="description">Description optionnelle</param>
        /// <param name="tags">Tags optionnels</param>
        /// <returns>L'entité image créée</returns>
        Task<ImageEntity> SaveImageAsync(IFormFile file, string? userId, string? description = null, string? tags = null);

        /// <summary>
        /// Récupère une image par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'image</param>
        /// <returns>L'image ou null si non trouvée</returns>
        Task<ImageEntity?> GetImageByIdAsync(Guid id);

        /// <summary>
        /// Récupère toutes les images d'un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Liste des images de l'utilisateur</returns>
        Task<IEnumerable<ImageEntity>> GetImagesByUserIdAsync(string userId);

        /// <summary>
        /// Met à jour les métadonnées d'une image
        /// </summary>
        /// <param name="id">Identifiant de l'image</param>
        /// <param name="description">Nouvelle description</param>
        /// <param name="tags">Nouveaux tags</param>
        /// <returns>L'image mise à jour ou null si non trouvée</returns>
        Task<ImageEntity?> UpdateImageMetadataAsync(Guid id, string? description, string? tags);

        /// <summary>
        /// Supprime une image
        /// </summary>
        /// <param name="id">Identifiant de l'image</param>
        /// <returns>True si supprimée, false sinon</returns>
        Task<bool> DeleteImageAsync(Guid id);
    }
}
