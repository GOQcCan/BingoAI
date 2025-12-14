using BingoAI.Server.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BingoAI.Server.Authorization;

/// <summary>
/// Handler d'autorisation vérifiant que l'utilisateur est propriétaire de l'image.
/// </summary>
public class ImageOwnerAuthorizationHandler : AuthorizationHandler<ImageOwnerRequirement, ImageEntity>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ImageOwnerRequirement requirement,
        ImageEntity resource)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);
        ArgumentNullException.ThrowIfNull(resource);

        if (context.User is null)
        {
            return Task.CompletedTask;
        }

        var userId = GetUserId(context.User);

        if (!string.IsNullOrEmpty(userId) && resource.UserId == userId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static string? GetUserId(ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Email)?.Value
        ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? user.FindFirst("sub")?.Value;
}