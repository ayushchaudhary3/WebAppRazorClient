using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebAppRazorClient.Models;

namespace WebAppRazorClient.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountService(IHttpClientFactory httpClientFactory) 
        {
            _httpClientFactory = httpClientFactory; 
        }

        public async Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient("ApiClient"); // Create an HttpClient instance //HttpClientFactory is preferred for better resource management

            using var resp = await client.PostAsJsonAsync("register", request, cancellationToken); // Send a POST request with the registration data as JSON

            if (!resp.IsSuccessStatusCode) // If the response indicates failure
            {
                string? content = null;
                try { content = await resp.Content.ReadAsStringAsync(cancellationToken); } catch { } // Read error response content if possible

                string? msg = null;
                try // Try to parse a structured error message from the response
                {
                    var obj = await resp.Content.ReadFromJsonAsync<ServerErrorResponse?>(cancellationToken: cancellationToken);
                    msg = obj?.Message;
                }
                catch { }

                var details = msg ?? content ?? $"Registration failed ({(int)resp.StatusCode})."; // Fallback error message
                return new RegisterResult(false, details); // Return failure result with details
            }

            try // Try to parse the success response body
            {
                var body = await resp.Content.ReadFromJsonAsync<ApiRegisterResponse?>(cancellationToken: cancellationToken);
                if (body is not null) return new RegisterResult(body.Success, body.Message);
            }
            catch { }

            return new RegisterResult(true, null); // Assume success if no body or parse error
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient("ApiClient"); // Create an HttpClient instance

            using var resp = await client.PostAsJsonAsync("login", request, cancellationToken); // Send a POST request with the login data as JSON
            
            var content = await resp.Content.ReadAsStringAsync(cancellationToken); // Read response content as string

            if (!resp.IsSuccessStatusCode) // If the response indicates failure
            {
                var details = string.IsNullOrWhiteSpace(content) ? $"Login failed ({(int)resp.StatusCode})." : content;
                return new LoginResult(false, null, null, null, details, null, null, null);
            }

            if (string.IsNullOrWhiteSpace(content))
                return new LoginResult(false, null, null, null, "Login succeeded but response did not contain a token.", null, null, null);

            // 1) Try direct deserialization to LoginResult (most APIs will match this)
            try
            {
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var direct = JsonSerializer.Deserialize<LoginResult?>(content, opts);
                if (direct is not null && !string.IsNullOrEmpty(direct.Token))
                    return direct;
            }
            catch
            {
                // ignore and try lightweight parsing
            }

            // 2) Minimal flexible parsing for common token property names (fallback)
            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                static string? GetString(JsonElement e, string name)
                    => e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;

                static int? GetInt(JsonElement e, string name)
                {
                    if (e.ValueKind != JsonValueKind.Object || !e.TryGetProperty(name, out var p)) return null;
                    if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var v)) return v;
                    if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), out var vs)) return vs;
                    return null;
                }

                // look for token in root (covers most APIs)
                string? token = GetString(root, "token")
                                ?? GetString(root, "access_token")
                                ?? GetString(root, "accessToken")
                                ?? GetString(root, "jwt")
                                ?? GetString(root, "id_token");

                if (!string.IsNullOrEmpty(token))
                {
                    var refresh = GetString(root, "refreshToken") ?? GetString(root, "refresh_token");
                    var tokenType = GetString(root, "tokenType") ?? GetString(root, "token_type");
                    var expiresIn = GetInt(root, "expiresIn") ?? GetInt(root, "expires_in");
                    var username = GetString(root, "username") ?? GetString(root, "userName") ?? GetString(root, "email");

                    string[]? roles = null;
                    if (root.TryGetProperty("roles", out var rp) && rp.ValueKind == JsonValueKind.Array)
                        roles = rp.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.String).Select(e => e.GetString()!).ToArray();

                    // also support nested "user" object with username/roles
                    if (root.TryGetProperty("user", out var u) && u.ValueKind == JsonValueKind.Object)
                    {
                        username ??= GetString(u, "username") ?? GetString(u, "userName") ?? GetString(u, "email");
                        if (roles == null && u.TryGetProperty("roles", out var ur) && ur.ValueKind == JsonValueKind.Array)
                            roles = ur.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.String).Select(e => e.GetString()!).ToArray();
                    }

                    return new LoginResult(true, token, username, roles, null, refresh, expiresIn, tokenType);
                }
            }
            catch
            {
                // ignore parse errors
            }

            var message = string.IsNullOrWhiteSpace(content) ? "Login succeeded but response did not contain a token." : content;
            return new LoginResult(false, null, null, null, message, null, null, null); // Response did not contain a token
        }

        // Helper classes for deserialization of API responses
        private sealed class ApiRegisterResponse 
        {
            public bool Success { get; set; } 
            public string? Message { get; set; } 
        }

        private sealed class ServerErrorResponse
        {
            public string? Message { get; set; } 
        }
    }
}