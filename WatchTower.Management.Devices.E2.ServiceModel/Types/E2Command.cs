using System.Runtime.Serialization;
using ServiceStack;
using ServiceStack.Html;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;
using WatchTower.Management.Devices.Shared;
using WatchTower.Management.Devices.Shared.Interfaces;

namespace WatchTower.Management.Devices.E2.ServiceModel.Types;

public abstract class E2Command<TRequest, TResponse, TResult> : DeviceCommand<TRequest, TResponse, TResult>, IHasLocationId
    where TRequest :  E2CommandRequest<TRequest, TResponse, TResult>
    where TResponse : E2CommandResponse<TResult> 
    where TResult : class, IHasResultData, new()
{
    protected override void SetMethod(string? value)
    {
        var requestName = typeof(TRequest).Name;
        
        _method = value ?? $"E2.{requestName}";
    }

    public override async Task ExecuteAsync(TRequest request)
    {
        try
        {
            var locationEndpoint = GetEndpointByLocationId(locationId: request.LocationId.Value);
            
            SetContentType( ContentType.ApplicationJson );
            SetEndpoint( locationEndpoint );
            SetMethod( $"E2.{typeof(TRequest).Name}");
        
            var requestBody = RequestFilter(request);

            var response = await HostContext.Resolve<HttpClient>().SendStringToUrlAsync(
                url: GetEndpoint().AbsoluteUri,
                method: "POST",
                requestBody: requestBody,
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
    protected override Uri GetEndpointByLocationId(int locationId)
    {
        var baseUrl = base.GetEndpointByLocationId(locationId);
        var result = new UriBuilder(baseUrl.Scheme, baseUrl.Host, 14106, "/JSON-RPC").Uri;
        
        return result;
    }

    protected override string CreatePayload(params object[] parameters) => new
    {
        Id = new Random().Next(1000000000, 2000000000),
        Method = GetMethod(),
        Params = parameters
    }
    .ToJson();

    protected abstract override TResult ResponseFilter(string json);


    public abstract int? LocationId { get; set; }
}


public abstract class E2CommandRequest<TRequest, TResponse, TResult> : E2Command<TRequest, TResponse, TResult> 
    where TRequest : E2CommandRequest<TRequest, TResponse, TResult> 
    where TResponse : E2CommandResponse<TResult> 
    where TResult : class, IHasResultData, new()
{
    [DataMember(Name = "id")]
    [Input(Type = Input.Types.Hidden)]
    public string Id { get; set; } = new Random().Next(1000000000, 2000000000).ToString();

    [DataMember(Name = "method")] 
    [Input(Type = Input.Types.Hidden)]
    public string Method { get; set; } = typeof(TRequest).Name;
    
    [DataMember(Name = "params")] 
    [Input(Type = Input.Types.Hidden)]
    public List<object>? Parameters { get; set; }

    public E2CommandRequest(params object[] parameters)
    {
        if (parameters.Length > 0)
            Parameters.AddRange(parameters);
    }
    
    public override string ToString()
    {
        return this.ToJson();
    }
}

public class E2CommandResponse<TResult> : IHasResult<TResult> where TResult : class, new()
{
    [DataMember(Name="id")]
    public string Id { get; set; }

    [DataMember(Name="jsonrpc")]
    public string JsonRPC { get; set; }

    [DataMember(Name = "result")] 
    public TResult Result { get; set; } = new();
}
