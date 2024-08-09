using System.Runtime.Serialization;
using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;
using WatchTower.Management.Devices.Shared;
using WatchTower.Management.Devices.Shared.Enums;
using WatchTower.Management.Devices.Shared.Types;

namespace WatchTower.Management.Devices.E2.ServiceModel.Types;

public abstract class E2Command<TRequest, TResponse, TResult> : DeviceCommand<TRequest, TResponse, TResult> 
    where TRequest : DeviceCommand<TRequest, TResponse, TResult> 
    where TResponse : class, IHasResult<TResult> 
    where TResult : class, IHasResultData, new()
{
    protected override void SetMethod(string? value)
    {
        var requestName = typeof(TRequest).Name;
        
        _method = value ?? $"E2.{requestName}";
    }

    protected override string CreatePayload(TRequest request)
    {
        var payload = request.ToJson();
        return payload;
    }

    public override async Task ExecuteAsync(TRequest request)
    {
        try
        {
            var locationEndpoint = GetEndpointByLocationId(locationId: request.LocationId);
            
            SetContentType( ContentType.TextPlain );
            SetEndpoint( locationEndpoint );
            SetMethod( $"E2.{typeof(TRequest).Name}");
            
            var response = await HostContext.Resolve<HttpClient>().SendStringToUrlAsync(
                url: GetEndpoint().AbsoluteUri,
                method: GetMethod(),
                requestBody: CreatePayload(request),
                contentType: GetContentType()
            );
            
            Result = ResponseFilter(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    protected abstract override TResult ResponseFilter(string json);
    
    protected override Uri GetEndpointByLocationId(int locationId)
    {
        var baseUrl = base.GetEndpointByLocationId(locationId);
        var result = new UriBuilder(baseUrl.Scheme, baseUrl.Host, 14106, "/JSON-RPC").Uri;
        
        return result;
    }

}