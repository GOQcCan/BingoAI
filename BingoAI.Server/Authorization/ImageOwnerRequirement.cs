using Microsoft.AspNetCore.Authorization;

namespace BingoAI.Server.Authorization;

/// <summary>
/// Requirement pour vérifier que l'utilisateur est propriétaire de l'image.
/// </summary>
public class ImageOwnerRequirement : IAuthorizationRequirement;