using ElevPortalen.Pages.AlertBox;
using ElevPortalen.Areas.Identity;
using ElevPortalen.Areas.Identity.Pages.Account;
using ElevPortalen.Data;
using ElevPortalen.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ElevPortalen.DatabaseErrorHandler;

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
//Job offer database
var JobOfferDataBase = builder.Configuration.GetConnectionString("JobOfferDataBase") ??
    throw new InvalidOperationException("Connection string 'LoginDatabase' not found.");

//DbContexts
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(LoginDatabase));
builder.Services.AddDbContext<ElevPortalenDataDbContext>(options => options.UseSqlServer(PortalDatabase));
builder.Services.AddDbContext<DataRecoveryDbContext>(options => options.UseSqlServer(DataRecoveryString));
builder.Services.AddDbContext<JobOfferDbContext>(options => options.UseSqlServer(JobOfferDataBase));

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
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<DawaService>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<AlertBox>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<RegisterModel>();
builder.Services.AddScoped<ImageUploadService>();
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

// Added database connection error handler
app.UseMiddleware<DatabaseErrorHandler>();

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

