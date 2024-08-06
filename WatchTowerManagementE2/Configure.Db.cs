using Microsoft.EntityFrameworkCore;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.PostgreSQL;
using WatchTowerManagementE2.ServiceInterface.Data;

[assembly: HostingStartup(typeof(WatchTowerManagementE2.ConfigureDb))]

namespace WatchTowerManagementE2;

public class ConfigureDb : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) => {

            var e2ManagerConnectionString      = context.Configuration.GetConnectionString("E2ManagerConnectionString");
            var primaryReadOnlyConnectionString = context.Configuration.GetConnectionString("PrimaryConnectionString");

            //IDbConnectionFactory dbConnectionFactory = new OrmLiteConnectionFactory(connectionString, PostgreSqlDialect.Provider);
            
            var dialect   = PostgreSqlDialectProvider.Instance;
            var dbFactory = new OrmLiteConnectionFactory( e2ManagerConnectionString, dialect );
            
            dbFactory.RegisterConnection( "Default",      e2ManagerConnectionString,      PostgreSqlDialectProvider.Instance );
            dbFactory.RegisterConnection( "PrimaryConnectionString", primaryReadOnlyConnectionString, PostgreSqlDialectProvider.Instance );
            
            services.AddSingleton<IDbConnectionFactory>( dbFactory );
            
            // $ dotnet ef migrations add CreateIdentitySchema
            // $ dotnet ef database update
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(e2ManagerConnectionString, b => b.MigrationsAssembly(nameof(WatchTowerManagementE2))));
            
            // Enable built-in Database Admin UI at /admin-ui/database
            services.AddPlugin(new AdminDatabaseFeature());
        });
}
