using System.Net;
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.Console;
using Hangfire.Heartbeat;
using Hangfire.Heartbeat.Server;
using Hangfire.JobsLogger;
using Hangfire.PostgreSql;
using Hangfire.RecurringJobAdmin;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using ServiceStack.Blazor;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using WatchTower.Management.Devices.E2.ServiceInterface;
using WatchTower.Management.Devices.E2.ServiceInterface.Data;
using WatchTower.Management.Devices.Manager;
using WatchTower.Management.Devices.Manager.Components;
using WatchTower.Management.Devices.Manager.Components.Account;
using WatchTower.Management.Devices.Manager.Jobs;
using WatchTower.Management.ServiceInterface;
using WatchTower.Management.ServiceModel.Types;

LogManager.LogFactory = new ConsoleLogFactory(true);

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var config = builder.Configuration;

// Add services to the container.
services.AddRazorComponents()
    .AddInteractiveServerComponents();

services.AddCascadingAuthenticationState();
services.AddScoped<IdentityUserAccessor>();
services.AddScoped<IdentityRedirectManager>();
services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies(options => options.DisableRedirectsForApis());
services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("App_Data"));

services.AddDatabaseDeveloperPageExceptionFilter();

services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
// Uncomment to send emails with SMTP, configure SMTP with "SmtpConfig" in appsettings.json
//services.AddSingleton<IEmailSender<ApplicationUser>, EmailSender>();
services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AdditionalUserClaimsPrincipalFactory>();

var baseUrl = builder.Configuration["ApiBaseUrl"] ??
    (builder.Environment.IsDevelopment() ? "https://localhost:5001" : "http://" + IPAddress.Loopback);
services.AddScoped(c => new HttpClient { BaseAddress = new Uri(baseUrl) });
services.AddBlazorServerIdentityApiClient(baseUrl);
services.AddLocalStorage();

JobStorage.Current = new PostgreSqlStorage(config.GetConnectionString("E2ManagerConnectionString"), 
    connection =>
    {
        connection.Open();
        connection.ChangeDatabase("watchtower_management_e2");
    }, new PostgreSqlStorageOptions
    {
        SchemaName = "jobs",
        PrepareSchemaIfNecessary = true
    }
);

services.AddHangfire((provider, configuration) =>
        {
            var factory = provider.GetService<IServiceScopeFactory>();

            configuration
                .UseJobsLogger(new JobsLoggerOptions()
                {
                    LogLevel = LogLevel.Debug
                })
                .UseConsole(new ConsoleOptions(){ 
                    BackgroundColor = "Blue", 
                    TextColor = "White",
                    
                })
                .UseRecurringJobAdmin(false, typeof(RecurringJobs).Assembly)
                .UseActivator(new AspNetCoreJobActivator(factory))
                .UseColouredConsoleLogProvider()
                .UseDashboardMetrics()
                .UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(1))
                ;
        })
        .AddHangfireServer((provider, options) =>
        {
            var factory = provider.GetService<IServiceScopeFactory>();

            options.Queues =
            [
                "default",
                "cache"
            ];
            options.Activator = new AspNetCoreJobActivator(factory);
        })
    ;

// Register all services
services.AddServiceStack([
    typeof( E2Service ).Assembly, 
    typeof( E2Service ).Assembly, 
    typeof( EmailServices ).Assembly
], o =>
{
    o.Services?.AddSingleton(c =>
    {
        var config = c.GetService<IAppSettings>()?.GetDictionary("SMTP");

        if (config is null) throw new InvalidOperationException("Unable to find AppSettings during initialization");
        
        return new SmtpConfig
        {
            FromEmail = config["From"],
            FromName = config["FromName"],
            Host = config["Host"],
            Username = config["UserName"],
            Password = config["Password"],
            Port = int.Parse(config["Port"])
        };
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();



app.UseServiceStack(new AppHost(), 
options => 
{
    options.MapEndpoints();
});

app.UseHangfireServer(() =>
{
    return new BackgroundJobServer
    (
        options: new BackgroundJobServerOptions(),
        additionalProcesses: [ new ProcessMonitor(checkInterval: TimeSpan.FromSeconds(1)) ],
        storage: JobStorage.Current
    );
});
app.UseHangfireDashboard("/admin-ui/jobs", new DashboardOptions()
{
    DarkModeEnabled      = false,
    DashboardTitle       = "Jobs",
    StatsPollingInterval = 1000
}, JobStorage.Current);


BlazorConfig.Set(new()
{
    Services = app.Services,
    JSParseObject = JS.ParseObject,
    IsDevelopment = app.Environment.IsDevelopment(),
    EnableLogging = app.Environment.IsDevelopment(),
    EnableVerboseLogging = app.Environment.IsDevelopment(),
});

app.Run();
