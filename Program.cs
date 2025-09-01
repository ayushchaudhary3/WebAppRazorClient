using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebAppRazorClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure authentication: cookie-based for the Razor client
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.Name = "WebAppRazorClient.Auth";
    });

// Accessor for reading claims in handler
builder.Services.AddHttpContextAccessor();

// DelegatingHandler that attaches access_token from the authenticated user's claims
builder.Services.AddTransient<TokenAttachHandler>();

// Named HttpClient to call your API (base URL read from configuration)
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"] ?? "https://localhost:7084/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<TokenAttachHandler>();

builder.Services.AddScoped<SandwichService>();
builder.Services.AddScoped<WebAppRazorClient.Service.StudentService>();
builder.Services.AddScoped<WebAppRazorClient.Service.CourseService>();
builder.Services.AddScoped<WebAppRazorClient.Service.EnrollmentService>();
// Register account service
builder.Services.AddScoped<WebAppRazorClient.Services.IAccountService, WebAppRazorClient.Services.AccountService>();

builder.Services.AddHttpClient("WebAPISandwich", httpclient =>
{
    httpclient.BaseAddress = new Uri("https://localhost:7084/api/Sandwich"); // fine to keep for sandwich controller calls
});

builder.Services.AddHttpClient("WebAPIStudent", httpclient =>
{
    httpclient.BaseAddress = new Uri("https://localhost:7113/api/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

// TokenAttachHandler implementation
public class TokenAttachHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenAttachHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var token = user.FindFirst("access_token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}