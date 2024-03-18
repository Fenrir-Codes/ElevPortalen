using ElevPortalen;
using ElevPortalen.Areas.Identity;
using ElevPortalen.Areas.Identity.Pages.Account;
using ElevPortalen.Data;
using ElevPortalen.Models;
using ElevPortalen.Pages.AlertBox;
using ElevPortalen.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//LoginDb
var LoginDatabase = builder.Configuration.GetConnectionString("LoginDatabase") ?? 
    throw new InvalidOperationException("Connection string 'LoginDatabase' not found.");
//The ElevPortalenDb.
var PortalDatabase = builder.Configuration.GetConnectionString("PortalDatabase") ?? 
    throw new InvalidOperationException("Connection string 'PortalDatabase' not found.");
//RecoveryDb
var DataRecoveryString = builder.Configuration.GetConnectionString("RecoveryDatabase") ??
    throw new InvalidOperationException("Connection string 'RecoveryDatabase' not found.");
//Message Database
var MessageDataString = builder.Configuration.GetConnectionString("MessageDatabase") ??
    throw new InvalidOperationException("Connection string 'MessageDatabase' not found.");

//DbContexts
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(LoginDatabase));
builder.Services.AddDbContext<ElevPortalenDataDbContext>(options => options.UseSqlServer(PortalDatabase));
builder.Services.AddDbContext<DataRecoveryDbContext>(options => options.UseSqlServer(DataRecoveryString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Added IdentityRole by Jozsef
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

//Services added by Jozsef
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<ElevPortalenDataDbContext>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<DawaService>();
builder.Services.AddScoped<RegisterModel>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<AlertBox>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddHttpClient();
//Dataprotection service by Jozsef
builder.Services.AddDataProtection();
// End ----

var app = builder.Build();

#region Check for Roles Existing
async Task CheckRolesExisting(WebApplication app)
{
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
}
#endregion

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

//Calling Check Task here
await CheckRolesExisting(app);

app.Run();
