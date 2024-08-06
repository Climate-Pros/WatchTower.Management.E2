using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Html;
using ServiceStack.Text;
using WatchTowerManagementE2.ServiceModel;
using WatchTowerManagementE2.ServiceModel.Commands.Enums;
using WatchTowerManagementE2.ServiceModel.Types;

namespace WatchTowerManagementE2.ServiceInterface.Commands.Types;

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
        var results = await HostContext.Resolve<HttpClient>().SendStringToUrlAsync(
            url: endpoint.ToString(),
            method: "POST",
            requestBody: CreatePayload(id, method, parameters.ToList()),
            contentType: "text/plain"
        );
        /*var results = await endpoint.ToString().PostStringToUrlAsync
        (
            contentType: "text/plain",
            requestBody: CreatePayload(id, method, parameters.ToList())
        );*/
            
        return func( results );
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
    
    protected async Task<T> ProfileCodeBlock<T>(string startText, Func<long, string> endText, Func<Task<T>> func)
    {
        startText.Print();
        var sw = new Stopwatch();
        sw.Start();
        var result = await func();
        sw.Stop();
        endText(sw.ElapsedMilliseconds).Print();

        return result;
    }
    
    protected async Task<T> ProfileCodeBlock<T>(string startText, Func<Task<T>> func)
    {
        startText.Print();

        return await func();
    }
}