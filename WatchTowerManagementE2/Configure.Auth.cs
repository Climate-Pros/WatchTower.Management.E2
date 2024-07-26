using ServiceStack.Auth;
using WatchTowerManagementE2.ServiceInterface.Data;

[assembly: HostingStartup(typeof(WatchTowerManagementE2.ConfigureAuth))]

namespace WatchTowerManagementE2;

public class ConfigureAuth : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services =>
        {
            services.AddPlugin(new AuthFeature(IdentityAuth.For<ApplicationUser>(options => {
                options.SessionFactory = () => new CustomUserSession();
                options.CredentialsAuth();
                options.AdminUsersFeature();
            })));
        });
}
