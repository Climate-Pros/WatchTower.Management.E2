using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Html;
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
    protected string CreatePayload(int id, string method, List<dynamic>? parameters = default)
    {
        var payload = new Payload(id, method, parameters ?? new List<object>()).ToJson();
        return payload;
    }

    public async Task<TResponse> ExecuteE2Command<TResponse>(Uri endpoint, string method, Func<string, TResponse> func, params object[] parameters )
    {
        return await ExecuteE2Command(endpoint, method, 0, func, parameters);
    }
    
    public async Task<TResponse> ExecuteE2Command<TResponse>(Uri endpoint, string method, int id, Func<string, TResponse> func, params object[] parameters )
    {
        var results = await endpoint.ToString().PostStringToUrlAsync
        (
            contentType: "text/plain",
            requestBody: CreatePayload(id, method, parameters.ToList())
        );
            
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
        
        scheme = Scheme.HTTP;
        host = location.IP;

        return new Uri($"{scheme}://{host}:14106/JSON-RPC");
    }
}