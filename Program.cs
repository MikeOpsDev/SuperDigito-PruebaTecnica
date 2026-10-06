using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperDigitoApp.Models;
using SuperDigitoApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<SuperDigitoDbContext>(options =>
    options.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=SuperDigitoDB;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddScoped<SuperDigitoService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
    });
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/login", async ([FromForm] string username, [FromForm] string password, HttpContext context, SuperDigitoDbContext db) =>
{
    var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);
    
    if (user != null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        
        return Results.Redirect("/");
    }
    
    return Results.Redirect("/login?error=true");
}).DisableAntiforgery();

app.MapPost("/api/auth/register", async ([FromForm] string username, [FromForm] string password, HttpContext context, SuperDigitoDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || string.IsNullOrWhiteSpace(password) || password.Length < 4) 
        return Results.Redirect("/login?error=invalid");
        
    if (await db.Usuarios.AnyAsync(u => u.Username == username)) 
        return Results.Redirect("/login?error=exists");
        
    var user = new Usuario { Username = username, PasswordHash = password };
    db.Usuarios.Add(user);
    await db.SaveChangesAsync();
    
    return Results.Redirect("/login?registered=true");
}).DisableAntiforgery();

app.MapPost("/api/auth/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).DisableAntiforgery();

app.MapRazorComponents<SuperDigitoApp.Components.App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SuperDigitoDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
