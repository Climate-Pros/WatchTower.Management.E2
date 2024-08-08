using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.CodeAnalysis.Options;
using ServiceStack;
using ServiceStack.Text;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;
using WatchTower.Management.Devices.Shared.Enums;
using WatchTower.Management.Devices.Shared.Types;

namespace WatchTower.Management.Devices.Shared;

public class DeviceCommand<TRequest, TResponse, TResult> : IAsyncCommand<TRequest,TResult>, IReturn<TResponse>
    where TRequest : class,  IReturn<TResponse>, new()
    where TResponse : class, IHasResult<TResult>
    where TResult : class, IHasResultData, new()

{
    protected ICommandExecutor CommandExecutor => HostContext.Resolve<ICommandExecutor>();
    private Scheme _scheme;
    private string _host;

    public TResult Result { get; set; }

    private string CreatePayload(int id, string method, List<dynamic>? parameters = default)
    {
        var payload = new Payload(id, method, parameters ?? new List<object>()).ToJson();
        return payload;
    }

    protected async Task<TResult> ExecuteCommand(Uri endpoint, string method,  object[] parameters, Func<string, TResult> results ) 
    {
        return await ExecuteCommand(endpoint, method, 0, results, parameters);
    }

    protected async Task<TResult> ExecuteCommand(Uri endpoint, string method, int id, Func<string, TResult> body, params object[] parameters )
    {
        try
        {   
            var response = await HostContext.Resolve<HttpClient>().SendStringToUrlAsync(
                url: endpoint.ToString(),
                method: "POST",
                requestBody: CreatePayload(id, method, parameters.ToList()),
                contentType: "text/plain"
            );


            var result = body(response);
            
            return result;
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

        return new($"{Scheme.HTTP}://{location.IP}:14106/JSON-RPC");
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