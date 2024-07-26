[assembly: HostingStartup(typeof(WatchTowerManagementE2.ConfigureOpenApi))]

namespace WatchTowerManagementE2;

public class ConfigureOpenApi : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) => {
            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen();

                services.AddServiceStackSwagger();
                //services.AddBasicAuth<Data.ApplicationUser>();
                //services.AddJwtAuth();
            
                services.AddTransient<IStartupFilter,StartupFilter>();
            }
        });

    public class StartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            next(app);
        };
    }
}
