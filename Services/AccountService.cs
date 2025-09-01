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
            var client = _httpClientFactory.CreateClient("ApiClient");

            using var resp = await client.PostAsJsonAsync("register", request, cancellationToken);

            if (!resp.IsSuccessStatusCode)
            {
                string? content = null;
                try { content = await resp.Content.ReadAsStringAsync(cancellationToken); } catch { }

                string? msg = null;
                try
                {
                    var obj = await resp.Content.ReadFromJsonAsync<ServerErrorResponse?>(cancellationToken: cancellationToken);
                    msg = obj?.Message;
                }
                catch { }

                var details = msg ?? content ?? $"Registration failed ({(int)resp.StatusCode}).";
                return new RegisterResult(false, details);
            }

            try
            {
                var body = await resp.Content.ReadFromJsonAsync<ApiRegisterResponse?>(cancellationToken: cancellationToken);
                if (body is not null) return new RegisterResult(body.Success, body.Message);
            }
            catch { }

            return new RegisterResult(true, null);
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            using var resp = await client.PostAsJsonAsync("login", request, cancellationToken);

            var content = await resp.Content.ReadAsStringAsync(cancellationToken);

            if (!resp.IsSuccessStatusCode)
            {
                var details = string.IsNullOrWhiteSpace(content) ? $"Login failed ({(int)resp.StatusCode})." : content;
                return new LoginResult(false, null, null, null, details, null, null, null);
            }

            // Parse body and extract token + refresh info (handles camelCase accessToken shape)
            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    using var doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;
                    string? token = null;
                    string? username = null;
                    string[]? roles = null;
                    string? refreshToken = null;
                    int? expiresIn = null;
                    string? tokenType = null;

                    // helper
                    static string? GetString(JsonElement e, string name)
                    {
                        return e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String
                            ? p.GetString() : null;
                    }

                    static int? GetInt(JsonElement e, string name)
                    {
                        if (e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var p))
                        {
                            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var v)) return v;
                            if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), out var vs)) return vs;
                        }
                        return null;
                    }

                    // candidates: root and common nested objects
                    var candidates = new System.Collections.Generic.List<JsonElement>();
                    if (root.ValueKind == JsonValueKind.Object) candidates.Add(root);
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("data", out var d) && d.ValueKind == JsonValueKind.Object) candidates.Add(d);
                        if (root.TryGetProperty("result", out var r) && r.ValueKind == JsonValueKind.Object) candidates.Add(r);
                        if (root.TryGetProperty("value", out var v) && v.ValueKind == JsonValueKind.Object) candidates.Add(v);
                    }

                    foreach (var c in candidates)
                    {
                        token ??= GetString(c, "token")
                                  ?? GetString(c, "access_token")
                                  ?? GetString(c, "accessToken")
                                  ?? GetString(c, "jwt")
                                  ?? GetString(c, "id_token");

                        refreshToken ??= GetString(c, "refreshToken") ?? GetString(c, "refresh_token");
                        tokenType ??= GetString(c, "tokenType") ?? GetString(c, "token_type");
                        expiresIn ??= GetInt(c, "expiresIn") ?? GetInt(c, "expires_in");

                        username ??= GetString(c, "username") ?? GetString(c, "userName") ?? GetString(c, "name") ?? GetString(c, "email");

                        if (roles == null && c.TryGetProperty("roles", out var rp) && rp.ValueKind == JsonValueKind.Array)
                        {
                            roles = rp.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.String).Select(e => e.GetString()!).ToArray();
                        }

                        if (c.TryGetProperty("user", out var u) && u.ValueKind == JsonValueKind.Object)
                        {
                            username ??= GetString(u, "username") ?? GetString(u, "userName") ?? GetString(u, "email");
                            if (roles == null && u.TryGetProperty("roles", out var ur) && ur.ValueKind == JsonValueKind.Array)
                                roles = ur.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.String).Select(e => e.GetString()!).ToArray();
                        }

                        if (!string.IsNullOrEmpty(token)) break;
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        return new LoginResult(true, token, username, roles, null, refreshToken, expiresIn, tokenType);
                    }
                }
            }
            catch
            {
                // ignore parse errors
            }

            var message = string.IsNullOrWhiteSpace(content) ? "Login succeeded but response did not contain a token." : content;
            return new LoginResult(false, null, null, null, message, null, null, null);
        }

        // Helper shapes (adjust to match your API JSON)
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