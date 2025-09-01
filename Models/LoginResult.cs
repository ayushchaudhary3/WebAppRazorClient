namespace WebAppRazorClient.Models
{
    // Extended to carry refresh token, expiresIn and tokenType returned by your API
    public sealed record LoginResult(
        bool Success,
        string? Token,
        string? Username,
        string[]? Roles,
        string? Message,
        string? RefreshToken,
        int? ExpiresIn,
        string? TokenType
    );
}