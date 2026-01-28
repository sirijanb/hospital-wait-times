using HQS.Infrastructure.Data;
using HQS.Infrastructure.Services;
using HQS.Infrastructure.Data.Seed;
using HQS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using HQS.Hubs;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Database
// --------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------
// Identity
// --------------------
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.Cookie.Name = "auth_token";
    options.LoginPath = "/login";
    options.Cookie.MaxAge = TimeSpan.FromMinutes(10);
    options.AccessDeniedPath = "/denied";
    options.SlidingExpiration = true;
});

// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.LoginPath = "/login";
//     options.AccessDeniedPath = "/denied";

//     options.Cookie.Name = "HQS.Auth";
//     options.Cookie.HttpOnly = true;
//     options.Cookie.SameSite = SameSiteMode.Lax;

//     options.SlidingExpiration = true;
// });


// --------------------
// Auth State Provider
// --------------------
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddHttpClient("ServerAPI", client =>
// {
//     client.BaseAddress = new Uri(builder.Configuration["ServerBaseUrl"] ?? "http://localhost:5126");
// })
// .ConfigurePrimaryHttpMessageHandler(() =>
//     new HttpClientHandler
//     {
//         UseCookies = true,
//         CookieContainer = new System.Net.CookieContainer()
//     })
// ;

/*
For address location
*/
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();   // ✅ ADD THIS

builder.Services.AddHttpClient("ServerAPI", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServerBaseUrl"] ?? "http://localhost:5126/");
});


builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<AuthStateProvider>());

builder.Services.AddScoped<IHospitalLocateService, HospitalLocateService>();
builder.Services.AddScoped<AddressState>();


// --------------------
// Blazor + Controllers
// --------------------    
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddControllers();

// --------------------
// Swagger
// --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Hospital Wait Times API",
        Version = "v1"
    });
});

// --------------------
// App Services
// --------------------
builder.Services.AddScoped<HospitalRepService>();
builder.Services.AddScoped<HospitalService>();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<HospitalHub>("/hubs/hospital");
app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

// --------------------
// Seed Roles & Admin
// --------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
  
    await RoleSeeder.SeedAsync(roleManager);
    
    await AdminSeeder.SeedAsync(userManager, roleManager);
    
    await HospitalSeeder.SeedAsync(db);
}
app.Run();
