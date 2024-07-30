using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using WatchTowerManagementE2.ServiceInterface;
using WatchTowerManagementE2.ServiceInterface.Commands.Types;
using Config = ServiceStack.Text.Config;

[assembly: HostingStartup(typeof(WatchTowerManagementE2.AppHost))]

namespace WatchTowerManagementE2;

public class AppHost() : AppHostBase("E2 Manager"), IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            services.AddHttpClient<HttpClient>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(30);
            });
        });

    public override void Configure()
    {
        Register(AppSettings);        
        
        using var db = Resolve<IDbConnectionFactory>().OpenDbConnection("ReadOnlyPrimary");

        using (JsConfig.With(new Config { IncludeTypeInfo = false, ExcludeDefaultValues = false, DateHandler = DateHandler.RFC1123 }))
        {
            ScriptContext.Args[nameof(GlobalAppData)] = new GlobalAppData
            {
                AllLocations = db.SqlList<Location>
                (@"SELECT 
                               wsl.id, 
                               CONCAT(wsl.name, '     ( ', REPLACE(wed.ip::TEXT, '/32','') ,':', wed.port::TEXT ,' )') as name, 
                               REPLACE(wed.ip::TEXT, '/32','') as ip, 
                               wed.port, 
                               wed.username, 
                               wed.password
                           FROM watchtower_schema_location wsl
                           INNER JOIN watchtower_einstein_device wed ON wed.location_id = wsl.id AND wed.gateway_device_id is NULL
                           GROUP BY wsl.id,
                                    wsl.name,
                                    REPLACE(wed.ip::TEXT, '/32',''), 
                               wed.port, 
                               wed.username, 
                               wed.password
                           ORDER BY wsl.id
                        ")
            };
        }

        SetConfig(new HostConfig
        {
        });
    }
}