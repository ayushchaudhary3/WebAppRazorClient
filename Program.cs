using WebAppRazorClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
//builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddScoped<SandwichService>();
builder.Services.AddScoped<WebAppRazorClient.Service.StudentService>();
builder.Services.AddScoped<WebAppRazorClient.Service.CourseService>();
builder.Services.AddScoped<WebAppRazorClient.Service.EnrollmentService>();

builder.Services.AddHttpClient("WebAPISandwich", httpclient =>
{
    httpclient.BaseAddress = new Uri("https://localhost:7084/api/Sandwich"); // Corrected BaseAddress
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();