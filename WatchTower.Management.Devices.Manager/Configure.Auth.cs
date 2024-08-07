using ServiceStack.Auth;
using WatchTower.Management.Devices.E2.ServiceInterface.Data;
using WatchTower.Management.Devices.Manager;

[assembly: HostingStartup(typeof(ConfigureAuth))]

namespace WatchTower.Management.Devices.Manager;

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
