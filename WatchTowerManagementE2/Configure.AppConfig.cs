using System.Security.Authentication;
using System.Text;
using Amazon;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Redis;
using ServiceStack.Text;
using WatchTowerManagementE2.Components;
using Environment = System.Environment;

[assembly: HostingStartup(typeof(ConfigureAppConfig))]

namespace WatchTowerManagementE2;

[Priority(-11)]
public class ConfigureAppConfig : IHostingStartup
{
    public static IAppSettings AppSettings;
    public static ILog Log = LogManager.GetLogger(typeof(ConfigureAppConfig));

    private IAmazonAppConfig _client;
    private IConfiguration Configuration { get; set; }
    
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureAppConfiguration( ( context, builder ) =>
        { ;
            AppSettings.Reload(context.Configuration);
            Configuration = ConfigureAwsAppConfig( context, builder );
            AppSettings.Reload(Configuration);
        } );
    
    private IConfiguration ConfigureAwsAppConfig( WebHostBuilderContext context, IConfigurationBuilder builder )
    {
        try
        {
            Log.Info( "Configuring App Configuration" );

            DotEnv.Load("~/.env");
            
            var environment = context.HostingEnvironment.EnvironmentName;
            var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION");
            
            AppSettings.Reload(context.Configuration);

            if (awsAccessKeyId is null || awsSecretAccessKey is null || awsRegion is null)
                throw new InvalidCredentialException("Missing AWS Secret Configuration. Supply the correct environment configuration to continue.");
            
            Log.Info( "Environment: {0}".FormatWith( environment ) );
            
            if (!environment.IsNullOrEmpty())
            {
                builder.Sources.Clear();

                var awsOptions = new AWSOptions { Credentials = new EnvironmentVariablesAWSCredentials(), Region = RegionEndpoint.USEast1 };
                
                builder
                    .AddAppConfig(applicationId: "WatchTower2", environmentId: "Global", configProfileId: "Global", awsOptions: awsOptions, optional: false, reloadAfter: TimeSpan.FromMinutes( 1 ) )
                    .AddAppConfig( applicationId: "WatchTower2", environmentId: environment, configProfileId: environment, awsOptions: awsOptions, optional: false, reloadAfter: TimeSpan.FromMinutes( 1 ) );

                builder
                    .AddJsonFile( "appsettings.json", true )
                    .AddJsonFile( $"appsettings.{environment}.json", true );
                
                builder
                    .AddEnvironmentVariables();

                Configuration = builder.Build();
                AppSettings.Reload(Configuration);

                Log.Info( "App Configuration configured" );
            }
            
            AppConstants.TimeToLiveExpiration = DateTime.UtcNow.AddSeconds( AppConstants.TimeToLiveInSeconds );

            Log.Info( $"Loaded AppConfig Data" );
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
        finally
        {
            Log.Info( "Finished Configuring App Configuration" );
        }

        return Configuration;
    }
}

public static class DotEnv
{
    public static void Load(string filePath)
    {
        var mappedFilePath = filePath.MapAbsolutePath();
        if (!File.Exists(mappedFilePath))
            return;

        foreach (var line in File.ReadAllLines(mappedFilePath))
        {
            if (line.IsNullOrEmpty()) continue;
            
            var parts = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;

            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}

public static class ConfigureAppConfigExtensions
{
    public static IAppSettings Reload(this IAppSettings appSettings, IConfiguration configuration)
    {
        return configuration.LoadAppSettings();
    }

    public static IAppSettings LoadAppSettings(this IConfiguration configuration)
    {
        DotEnv.Load("~/.env".MapAbsolutePath());

        configuration = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddEnvironmentVariables()
            .Build();
            
        var result = ConfigureAppConfig.AppSettings = new NetCoreAppSettings(configuration);

        return result;
    }
}

public class AppConfigDataService : IAppConfigDataService
{
    private IAppSettings _appSettings;
    private static ILog _log = LogManager.GetLogger(typeof(AppConfigDataService));
    private RedisManagerPool _redisManagerPool;
    private string _applicationName;
    private readonly Guid _clientId;

    public AppConfigDataService(IAppSettings appSettings, Guid clientId, string applicationName)
    {
        var redisConnectionString = appSettings.GetConnectionString("RedisCache");

        _appSettings = appSettings;
        _clientId = clientId;
        _applicationName = applicationName.Trim();
        _redisManagerPool = new(redisConnectionString);
    }

    public async Task<AppConfigData> GetAppConfigData()
    {
        try
        {
            _log.Info("--------- Executing GetAppConfigData ------- ");

            AppConfigData config = null;

            var cacheKey = $"AppConfig:{_appSettings.GetString("ASPNETCORE_ENVIRONMENT")}:{_applicationName}";

            using var redisClient = _redisManagerPool.GetCacheClient();
            config = redisClient.Get<AppConfigData>(cacheKey);

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Local" ||
                (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local" && config == null))
            {
                _log.Info("Unable to find cached configuration settings, refreshing");

                "CALLED GetConfigurationAPI to get AppConfigData".Print();

                var appConfigService = new AppConfigService(_clientId);
                var getConfigurationResponse = await appConfigService.GetConfigurationResponse().ConfigAwait();

                AppConstants.ClientConfigurationVersion = getConfigurationResponse.ConfigurationVersion;

                var decodedResponseData = getConfigurationResponse.Content.Length > 0
                    ? MemoryStreamHelper.DecodeMemoryStreamToString(getConfigurationResponse.Content)
                    : string.Empty;


                config = ConvertDecodedResponseToAppConfigData(decodedResponseData);

                var cachedValue = redisClient.Set(cacheKey, config, TimeSpan.FromDays(1));

                if (cachedValue)
                    _log.Info("Cached configuration that retrieved from GetConfigurationAPI response");
                else
                    throw new InvalidOperationException("Caching Of GetConfigurationAPI response FAILED!");
            }
            else
            {
                _log.Info("Returning cached configuration");
            }

            AppConstants.AppConfigData = config;

            _log.Info(" --------- Exiting GetAppConfigData (Fresh AppConfigData) ------- ");

            return config;
        }
        catch (Exception ex)

        {
            _log.Error(ex.ToString());
        }

        _log.Info(" --------- Exiting GetAppConfigData (Cached AppConfigData) ------- ");

        return AppConstants.AppConfigData;
    }

    private AppConfigData ConvertDecodedResponseToAppConfigData(string decodedResponseData) =>
        string.IsNullOrEmpty(decodedResponseData)
            ? AppConstants.AppConfigData
            : decodedResponseData.FromJson<AppConfigData>();
}

public static class AppConstants
{
    public static string AppConfigApplication = "WatchTower2";

    public static string AppConfigEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    public static string AppConfigConfiguration = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    public static string ClientConfigurationVersion;

    public static DateTime TimeToLiveExpiration;

    public static double TimeToLiveInSeconds = 900;

    public static AppConfigData AppConfigData;

    public static GetConfigurationResponse GetConfigurationResponse;
}

public class AppConfigData
{
    public Dictionary<string, string> AWS { get; set; }

    public Dictionary<string, string> ConnectionStrings { get; set; }

    public Dictionary<string, string> Sentry { get; set; }

    public string ApiToken { get; set; }

    public string WebHost { get; set; }
}

public interface IAppConfigDataService
{
    Task<AppConfigData> GetAppConfigData();
}

public interface IAppConfigService
{
    Task<GetConfigurationResponse> GetConfigurationResponse();
}

public class AppConfigService : IAppConfigService
{
    private readonly Guid _clientId;

    public AppConfigService(Guid clientId = new())
    {
        _clientId = clientId;
    }

    private static ILog Log => LogManager.LogFactory.GetLogger(typeof(AppConfigService));

    public async Task<GetConfigurationResponse> GetConfigurationResponse()
    {
        var awsAppConfigClient = new AmazonAppConfigClient();

        var request = new GetConfigurationRequest
        {
            Application = AppConstants.AppConfigApplication,
            Environment = AppConstants.AppConfigEnvironment,
            Configuration = AppConstants.AppConfigConfiguration,
            ClientId = _clientId.ToString(),
            ClientConfigurationVersion = AppConstants.ClientConfigurationVersion
        };

        Log.Info("Retrieving AppConfig Configuration");

        var results = await awsAppConfigClient.GetConfigurationAsync(request).ConfigAwait();

        Log.Info(" -- done ");

        return results;
    }
}

public static class MemoryStreamHelper
{
    public static string DecodeMemoryStreamToString(MemoryStream content)
    {
        var result = string.Empty;
        int count;
        var uniEncoding = new UnicodeEncoding();

        using (var memoryStream = content)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            var byteArray = new byte[memoryStream.Length];
            count = memoryStream.Read(byteArray, 0, 20);

            while (count < memoryStream.Length) byteArray[count++] = Convert.ToByte(memoryStream.ReadByte());

            var charArray = new char[uniEncoding.GetCharCount(byteArray, 0, count)];

            uniEncoding.GetDecoder().GetChars(byteArray, 0, count, charArray, 0);

            var decodedBytes = Convert.FromBase64String(Convert.ToBase64String(Encoding.Unicode.GetBytes(charArray)));
            result = Encoding.UTF8.GetString(decodedBytes);
        }

        return result;
    }
}