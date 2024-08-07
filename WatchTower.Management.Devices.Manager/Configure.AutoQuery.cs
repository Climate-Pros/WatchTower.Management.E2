using ServiceStack.Data;
using WatchTower.Management.Devices.E2.ServiceModel;
using WatchTower.Management.Devices.Manager;

[assembly: HostingStartup(typeof(ConfigureAutoQuery))]

namespace WatchTower.Management.Devices.Manager;

public class ConfigureAutoQuery : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            // Enable Audit History
            services.AddSingleton<ICrudEvents>(c =>
                new OrmLiteCrudEvents(c.GetRequiredService<IDbConnectionFactory>()));

            // For TodosService
            services.AddPlugin(new AutoQueryDataFeature());

            // For Bookings https://docs.servicestack.net/autoquery-crud-bookings
            services.AddPlugin(new AutoQueryFeature
            {
                MaxLimit = 1000,
                //IncludeTotal = true,
            });
            
            services.AddPlugin(
                new AutoQueryDataFeature()
                    .AddDataSource(context => context.ServiceSource<QueryApplicationTypes>(new QueryApplicationTypes(context)))
            );
        })
        .ConfigureAppHost(appHost => {
            appHost.Resolve<ICrudEvents>().InitSchema();
        });
}