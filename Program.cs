using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Microsoft.AspNetCore.Identity;
using Marketplace_LabWebBD.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configurar cultura para pt-PT
var cultureInfo = new CultureInfo("pt-PT");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar o DbContext com SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar ASP.NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;

    // SignIn settings
    options.SignIn.RequireConfirmedEmail = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Token expiration (24 hours for email confirmation)
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(24);
});

// Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

// Admin Logging Service
builder.Services.AddScoped<IAdminLogService, AdminLogService>();

// Anuncio Service
builder.Services.AddScoped<IAnuncioService, AnuncioService>();

// Image Upload Service
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

// Search Service
builder.Services.AddScoped<ISearchService, SearchService>();

// Reserva Service
builder.Services.AddScoped<IReservaService, ReservaService>();

// Background Service para expirar reservas
builder.Services.AddHostedService<ReservaExpirationHostedService>();

// Visita Service
builder.Services.AddScoped<IVisitaService, VisitaService>();

// Compra Service
builder.Services.AddScoped<ICompraService, CompraService>();

// Promoção Admin Service
builder.Services.AddScoped<IPromocaoAdminService, PromocaoAdminService>();

// Filtro Favorito Service
builder.Services.AddScoped<IFiltroFavoritoService, FiltroFavoritoService>();

// Statistics Service
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

// Configurar RequestLocalization para aceitar formatos de data ISO
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("pt-PT"), new CultureInfo("en-US") };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-PT");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Usar localização
app.UseRequestLocalization();

// Inicializar base de dados (marcas e modelos)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
