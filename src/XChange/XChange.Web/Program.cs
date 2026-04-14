using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using XChange.Core.Interfaces;
using XChange.Infrastructure.Data;
using XChange.Infrastructure.Repositories;
using XChange.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no se encontró.");

// Inyección de Dependencias (DI).
builder.Services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));

// Repositorios.
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Servicios de Seguridad Externos.
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

// Configuración de Autenticación (Cookies + Google OAuth).
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Auth/Login";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
        ?? throw new InvalidOperationException("Falta el ClientId de Google.");
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
        ?? throw new InvalidOperationException("Falta el ClientSecret de Google.");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Landing}");

app.Run();