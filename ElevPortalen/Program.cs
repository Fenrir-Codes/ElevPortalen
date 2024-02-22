using ElevPortalen.Areas.Identity;
using ElevPortalen.Areas.Identity.Pages.Account;
using ElevPortalen.Data;
using ElevPortalen.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//The ElevPortalen-Data db string.
var dbString = builder.Configuration.GetConnectionString("DbConnection") ?? 
    throw new InvalidOperationException("Connection string 'DbConnection' not found.");

var DataRecoveryDbString = builder.Configuration.GetConnectionString("RecoveryDbConnection") ??
    throw new InvalidOperationException("Connection string 'DbConnection' not found.");

builder.Services.AddDbContext<ElevPortalenDataDbContext>(options => options.UseSqlServer(dbString));
builder.Services.AddDbContext<DataRecoveryDbContext>(options => options.UseSqlServer(DataRecoveryDbString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Added IdentityRole by Jozsef
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();

//Services added by Jozsef
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<ElevPortalenDataDbContext>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<DawaService>();
builder.Services.AddScoped<RegisterModel>();
builder.Services.AddHttpClient();
//End ----

//added Database context by Jozsef
builder.Services.AddDbContext<ElevPortalenDataDbContext>(options =>
    options.UseSqlServer(connectionString));
// End ----

//Dataprotection service by Jozsef
builder.Services.AddDataProtection();
// End ----


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
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

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Create a new scope within the dependency injection container by Jozsef
using (var scope = app.Services.CreateScope())
{
    // Obtain an instance of the RoleManager service for managing roles
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Define an array of role names to be created and seeded
    var roles = new[] { "Admin", "Student", "Company" };

    // Iterate over each role name in the array
    foreach (var role in roles)
    {
        // Check if the role already exists in the system
        if (!await roleManager.RoleExistsAsync(role))
        {
            // If the role does not exist, create a new role using RoleManager
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.Run();
