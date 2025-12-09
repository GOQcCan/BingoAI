using BingoAI.Server.Authorization;
using BingoAI.Server.DTOs;
using BingoAI.Server.Models;
using BingoAI.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
    
namespace BingoAI.Server.Controllers;

/// <summary>
/// Contrôleur API pour la gestion des images.
/// Suit le principe de responsabilité unique (SRP) - gère uniquement les endpoints HTTP.
/// Suit le principe d'inversion des dépendances (DIP) - dépend de IImageService.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImageController(
    IImageService imageService,
    IAuthorizationService authorizationService,
    ILogger<ImageController> logger) : ControllerBase
{
    private static readonly ImageOwnerRequirement OwnerRequirement = new();

    /// <summary>
    /// Upload une nouvelle image
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImageMetadataDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadImage(IFormFile file, string? description, string? tags)
    {
        try
        {
            var userId = GetCurrentUserId();
            var image = await imageService.SaveImageAsync(file, userId, description, tags);

            return CreatedAtAction(nameof(GetImageMetadata), new { id = image.Id }, MapToDto(image));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Image upload validation failed: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Récupère les métadonnées d'une image
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ImageMetadataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetImageMetadata(Guid id)
    {
        var image = await imageService.GetImageByIdAsync(id);

        if (image is null)
        {
            return NotFound(new { error = $"Image avec l'ID {id} non trouvée." });
        }

        if (!await IsOwnerAsync(image))
        {
            return Forbid();
        }

        return Ok(MapToDto(image));
    }

    /// <summary>
    /// Télécharge les données binaires d'une image
    /// </summary>
    [HttpGet("{id:guid}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DownloadImage(Guid id)
    {
        var image = await imageService.GetImageByIdAsync(id);

        if (image is null)
        {
            return NotFound(new { error = $"Image avec l'ID {id} non trouvée." });
        }

        if (!await IsOwnerAsync(image))
        {
            return Forbid();
        }

        return File(image.Data, image.ContentType, image.FileName);
    }

    /// <summary>
    /// Récupère toutes les images de l'utilisateur connecté
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ImageMetadataDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyImages()
    {
        var userId = GetCurrentUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "Utilisateur non identifié." });
        }

        var images = await imageService.GetImagesByUserIdAsync(userId);

        return Ok(images.Select(MapToDto));
    }

    /// <summary>
    /// Met à jour les métadonnées d'une image
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ImageMetadataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateImageMetadata(Guid id, [FromBody] ImageUpdateDto dto)
    {
        var existingImage = await imageService.GetImageByIdAsync(id);

        if (existingImage is null)
        {
            return NotFound(new { error = $"Image avec l'ID {id} non trouvée." });
        }

        if (!await IsOwnerAsync(existingImage))
        {
            return Forbid();
        }

        var updatedImage = await imageService.UpdateImageMetadataAsync(id, dto.Description, dto.Tags);

        return Ok(MapToDto(updatedImage!));
    }

    /// <summary>
    /// Supprime une image
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteImage(Guid id)
    {
        var existingImage = await imageService.GetImageByIdAsync(id);

        if (existingImage is null)
        {
            return NotFound(new { error = $"Image avec l'ID {id} non trouvée." });
        }

        if (!await IsOwnerAsync(existingImage))
        {
            return Forbid();
        }

        await imageService.DeleteImageAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Vérifie si l'utilisateur courant est propriétaire de l'image via le handler d'autorisation.
    /// </summary>
    private async Task<bool> IsOwnerAsync(ImageEntity image)
    {
        var result = await authorizationService.AuthorizeAsync(User, image, OwnerRequirement);
        return result.Succeeded;
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
    }

    private static ImageMetadataDto MapToDto(ImageEntity image) => new()
    {
        Id = image.Id,
        FileName = image.FileName,
        ContentType = image.ContentType,
        FileSize = image.FileSize,
        CreatedAt = image.CreatedAt,
        UpdatedAt = image.UpdatedAt,
        Description = image.Description,
        Tags = image.Tags
    };
}
