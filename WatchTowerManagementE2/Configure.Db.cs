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

            var defaultConnection      = context.Configuration.GetConnectionString("DefaultConnection");
            var readonlyPrimaryConnection = context.Configuration.GetConnectionString("ReadOnlyPrimary");

            //IDbConnectionFactory dbConnectionFactory = new OrmLiteConnectionFactory(connectionString, PostgreSqlDialect.Provider);
            
            var dialect   = PostgreSqlDialectProvider.Instance;
            var dbFactory = new OrmLiteConnectionFactory( defaultConnection, dialect );
            
            dbFactory.RegisterConnection( "Default",      defaultConnection,      PostgreSqlDialectProvider.Instance );
            dbFactory.RegisterConnection( "ReadOnlyPrimary", readonlyPrimaryConnection, PostgreSqlDialectProvider.Instance );
            
            services.AddSingleton<IDbConnectionFactory>( dbFactory );
            
            // $ dotnet ef migrations add CreateIdentitySchema
            // $ dotnet ef database update
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(defaultConnection, b => b.MigrationsAssembly(nameof(WatchTowerManagementE2))));
            
            // Enable built-in Database Admin UI at /admin-ui/database
            services.AddPlugin(new AdminDatabaseFeature());
        });
}
