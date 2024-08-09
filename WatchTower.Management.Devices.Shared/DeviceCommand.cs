using System.Diagnostics;
using System.Net.Sockets;
using ServiceStack;
using ServiceStack.Text;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;
using WatchTower.Management.Devices.Shared.Enums;
using WatchTower.Management.Devices.Shared.Interfaces;
using WatchTower.Management.Devices.Shared.Types;

namespace WatchTower.Management.Devices.Shared;

public class ContentType
{
    public static string ApplicationJson => "application/json";
    public static string TextPlain => "text/plain";
    public static string ApplicationWWWFormUrlEncoded => "application/x-www-form-urlencoded";
    public static string ApplicationWWWFormUrlEncodedAsUtf8 => "application/x-www-form-urlencoded; charset=UTF-8";
}

public abstract class DeviceCommand<TRequest, TResponse, TResult> : IAsyncCommand<TRequest,TResult>, IReturn<TResponse>
    where TRequest : class //DeviceCommand<TRequest, TResponse, TResult>
    where TResponse : class, IHasResult<TResult>
    where TResult : class, IHasResultData, new()

{
    protected ICommandExecutor CommandExecutor => HostContext.Resolve<ICommandExecutor>();
    private Scheme _scheme;
    private string _host;

    public TResult Result { get; set; }

    protected internal string _contentType;
    protected internal string _method;
    protected internal Uri _endpoint;
    
    protected abstract void SetMethod(string value);


    protected string GetMethod()
    {
        return _method;
    }

    protected void SetEndpoint( string value )
    {
        _endpoint = new Uri(value);
    }
    
    protected void SetEndpoint( Uri value )
    {
        _endpoint = value;
    }

    protected Uri GetEndpoint()
    {
        return _endpoint;
    }
    
    protected void SetContentType( ContentType value )
    {
        _contentType = value.ToString();
    }
    
    protected void SetContentType( string customValue )
    {
        _contentType = customValue;
    }

    protected string GetContentType()
    {
        return _contentType;
    }

    protected abstract string CreatePayload(params object[] parameters);
    /*{
        throw new InvalidOperationException("FATAL: Override the CreatePayload method");        
    }*/

    /*protected async Task<TResult> ExecuteCommand( string contentType, string endpoint, string method, TRequest request, Func<string, TResult> responseFilter)
    {
        var response = await HostContext.Resolve<HttpClient>().SendStringToUrlAsync(
            url: endpoint,
            method: "POST",
            requestBody: RequestFilter(request),
            contentType: contentType
        );
        
        var result = responseFilter(response);
        
        return result;
    }*/

    public virtual async Task ExecuteAsync(TRequest request)
    {
        /*
        var response = await ExecuteCommand
        (
            request: request,
            responseFilter: json => json.FromJson<GetControllerListResult>()
        ); 

        */
        //Result = await ExecuteCommand(request, ResponseFilter);
        ;
    }

    protected virtual string RequestFilter(TRequest request)
    {
        return CreatePayload([1]);
    }
    protected abstract TResult ResponseFilter(string json);

    protected Dictionary<int, string> _endpointCache = new(); 

    protected virtual Uri GetEndpointByLocationId(int locationId)
    {
        var ip = "";

        if (_endpointCache.TryGetValue(locationId, out var value))
            ip = value;
        else
        {
            var globalAppData = (GlobalAppData)HostContext.AppHost.ScriptContext.Args[nameof(GlobalAppData)];
            var location = globalAppData.AllLocations.FirstOrDefault(_ => _.Id == locationId);

            if (location is null)
                throw new InvalidOperationException($"Unable to find a location with an id: {locationId}");

            _endpointCache.TryAdd(locationId, location.IP);
            
            ip = location.IP;
        }
        
        return new Uri($"{Scheme.HTTP}://{ip}");
    }
    
}

/// <summary>
/// Working on this Profile subset
/// </summary>
public static class DeviceCommandExtensions
{
    public static async Task<bool> IsOnline(this Uri endpoint,  TimeSpan? timeout = default)
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

        return await request.Item2();
    }   
    
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