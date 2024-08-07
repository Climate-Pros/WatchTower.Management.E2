using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Html;
using ServiceStack.Text;
using WatchTower.Management.Devices.E2.ServiceModel;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.Enums;
using WatchTower.Management.Devices.E2.ServiceModel.Types;

namespace WatchTower.Management.Devices.E2.ServiceInterface.Commands.Types;

public class E2Command<TRequest, TResult> : IAsyncCommand<TRequest, TResult>  where TRequest : class, new()
{
    protected ICommandExecutor CommandExecutor => HostContext.Resolve<ICommandExecutor>();
    public TResult Result { get; set; }
    public Uri Endpoint { get; protected set; }

    protected Scheme scheme;
    protected string host;

    private List<ApplicationType>? _applicationTypes = default;
    private List<ApplicationGroup>? _applicationGroups = default;
    
    protected List<ApplicationType> ApplicationTypes
    {
        get
        {
            if (_applicationTypes is null)
                _applicationTypes = ((GetApplicationTypesResponse)HostContext.AppHost.ExecuteService(new GetApplicationTypes())).Result;

            return _applicationTypes;
        }
    }

    protected List<ApplicationGroup> ApplicationGroups
    {
        get
        {
            if (_applicationGroups is null)
                _applicationGroups =  ((GetApplicationGroupsResponse)HostContext.AppHost.ExecuteService(new GetApplicationGroups())).Result;

            return _applicationGroups;
        }
    }

    private string CreatePayload(int id, string method, List<dynamic>? parameters = default)
    {
        var payload = new Payload(id, method, parameters ?? new List<object>()).ToJson();
        return payload;
    }

    protected async Task<TResponse> ExecuteE2Command<TResponse>(Uri endpoint, string method, Func<string, TResponse> func, params object[] parameters )
    {
        return await ExecuteE2Command(endpoint, method, 0, func, parameters);
    }

    protected async Task<TResponse> ExecuteE2Command<TResponse>(Uri endpoint, string method, int id, Func<string, TResponse> func, params object[] parameters )
    {
        try
        {
             
            
            var results = await HostContext.Resolve<HttpClient>().SendStringToUrlAsync(
                url: endpoint.ToString(),
                method: "POST",
                requestBody: CreatePayload(id, method, parameters.ToList()),
                contentType: "text/plain"
            );
            
            return func( results );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

    }
    
    public virtual Task ExecuteAsync(TRequest request)
    {
        return Task.CompletedTask;
    }

    protected Uri GetEndpointByLocationId(int locationId)
    {
        var globalAppData = (GlobalAppData)HostContext.AppHost.ScriptContext.Args[nameof(GlobalAppData)];
        var location = globalAppData.AllLocations.FirstOrDefault(_ => _.Id == locationId);
        
        if (location is null)
            throw new InvalidOperationException($"Unable to find a location with an id: {locationId}");
        else
        {
            scheme = Scheme.HTTP;
            host = location.IP;

            return new Uri($"{scheme}://{host}:14106/JSON-RPC");            
        }
    }
}

public static class E2CommandExtensions
{/*
    public static bool IsOnline(this Uri endpoint,  TimeSpan? timeout = default)
    {
        var request = new Tuple<string, Func<Task<bool>>>("Starting controller availability test", async  () =>
        {

            timeout ??= TimeSpan.FromSeconds(2);

            try
            {
                var host = endpoint.Host;
                var port = endpoint.Port;

                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne((TimeSpan)timeout);
                    client.EndConnect(result);
                    return success;
                }
            }
            catch
            {
                return false;
            }
        });

        return request.Profile();
    }   */
    
    /*public static async Task Profile(this (string, Func<long, string>) parameters, Func<Task> profile)
    {
        var startText = parameters.Item1;
        var endText = parameters.Item2;
        
        startText.Print();
        var sw = new Stopwatch();
        sw.Start();
        var result = await profile();
        sw.Stop();
        endText(sw.ElapsedMilliseconds).Print();

        return result;
    }*/
    
    public static async Task<T> Profile<T>(this (string, Func<long, string>) parameters, Func<Task<T>> profile)
    {
        var startText = parameters.Item1;
        var endText = parameters.Item2;
        
        startText.Print();
        var sw = new Stopwatch();
        sw.Start();
        var result = await profile();
        sw.Stop();
        endText(sw.ElapsedMilliseconds).Print();

        return result;
    }
    
    public static T Profile<T>(this (string, Func<T>) parameters)
    {
        var startText = parameters.Item1;
        var profile = parameters.Item2;

        startText.Print();

        return  profile();
    }
    
}