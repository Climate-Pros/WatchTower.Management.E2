using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using WatchTower.Management.Devices.E2.ServiceInterface;
using WatchTower.Management.Devices.E2.ServiceInterface.Commands.Types;
using WatchTower.Management.Devices.Manager;
using WatchTower.Management.Devices.Manager.Migrations;
using Config = ServiceStack.Text.Config;

[assembly: HostingStartup(typeof(AppHost))]

namespace WatchTower.Management.Devices.Manager;

public class AppHost() : AppHostBase("Device Manager"), IHostingStartup
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
        
        using var db = Resolve<IDbConnectionFactory>().OpenDbConnection("PrimaryConnectionString");

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
            
            ScriptContext.Args[nameof(GlobalAppData)].PrintDumpTable();
        }

        SetConfig(new HostConfig
        {
        });
        
        var migrator = new Migrator(this.Resolve<IDbConnectionFactory>(), typeof(Migration1000).Assembly);
        migrator.Run();
    }
}