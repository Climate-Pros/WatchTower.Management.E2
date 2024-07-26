/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Funq;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Hangfire.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;

namespace WatchTowerManagementE2
{
    [Priority(-1)]
    public class ConfigureHangfirePostgreSql : IConfigureServices, IConfigureApp
    {
        IConfiguration Configuration { get; }
        public ConfigureHangfirePostgreSql(IConfiguration configuration) => Configuration = configuration;
        public void Configure(IServiceCollection services)
        {
            var conn = Configuration["database:connectionString"];
            
            services.AddHangfire((isp, config) =>
            {
                config.UsePostgreSqlStorage(conn, new PostgreSqlStorageOptions
                {
                    InvisibilityTimeout = TimeSpan.FromDays(1)
                });

                config.UseConsole();

            });

            //all cron services must be registered in IoC
            services.AddSingleton<TestCron>();
        }

        public void Configure(IApplicationBuilder app)
        {
            //needs to be called before servicestack or route will be unreachable
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });
        }
    }

    //priority 100 so it runs after all services are registered
    [Priority(100)]
    public class ConfigureHangfirePostgreSqlPost : IConfigureApp
    {
        public void Configure(IApplicationBuilder app)
        {
            //Join IoC so hangfire can autowire cronjob classes
            Hangfire.GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(app.ApplicationServices));

            //register crons
            var cronStatus = Environment.GetEnvironmentVariable("DISABLE_CRON");
            if (string.IsNullOrWhiteSpace(cronStatus) || cronStatus.ToLower() == "false" || cronStatus == "0")
            {
                RecurringJob.AddOrUpdate<TestCron>(x => x.Start(null), "* * * * *");
                //add more here...
            }

            app.UseHangfireServer();
        }

    }
    /// <summary>
    /// Check user session for admin role and grant access if found.  If development mode always grant access
    /// </summary>
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                return true;
            }

            var coreContext = context.GetHttpContext();
            var httpContext = coreContext.ToRequest();
            var session = httpContext.GetSession();

            if (session != null && session.IsAuthenticated && session.Roles != null && session.Roles.Contains("Admin"))
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Gives services from IoC to Hangfire activator
    /// </summary>
    public class ContainerJobActivator : JobActivator
    {
        private IServiceProvider _container;

        public ContainerJobActivator(IServiceProvider container)
        {
            _container = container;
        }

        public override object ActivateJob(Type type)
        {
            return _container.GetService(type);
        }
    }

    /// <summary>
    /// Simple example of cron with output console and progress bar
    /// </summary>
    public class TestCron
    {
        public void Start(PerformContext context)
        {
            context.WriteLine("Starting");
            // create progress bar
            var progress = context.WriteProgressBar();
            progress.SetValue(33);
            Thread.Sleep(5000);
            progress.SetValue(77);
            Thread.Sleep(5000);
            progress.SetValue(100);
            Thread.Sleep(5000);
            context.WriteLine("Finished");
        }
    }
}
*/
