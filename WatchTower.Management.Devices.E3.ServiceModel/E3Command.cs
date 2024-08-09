using System.Runtime.Serialization;
using ServiceStack;
using ServiceStack.Html;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;
using WatchTower.Management.Devices.Shared;

namespace WatchTower.Management.Devices.E3.ServiceModel;

[DataContract(Name="Params")]
public class E3CommandParameters
{
    [DataMember(Name="sid")]
    public string Sid { get; set; }
}

public abstract class E3Command<TRequest, TResponse, TResult> : DeviceCommand<TRequest, TResponse, TResult> 
    where TRequest : E3CommandRequest<TRequest, TResponse, TResult>
    where TResponse : E3CommandResponse<TResult>
    where TResult : class, IHasResultData, new()
{
    protected override void SetMethod(string? value)
    {
        if (value is not null)
            _method = value;

        _method = typeof(TRequest).Name;
    }

    protected override string CreatePayload(params object[] parameters)
    {
        return $"m={parameters.ToJson()}";
    }

    public override async Task ExecuteAsync(TRequest request)
    {
        try
        {
            var locationEndpoint = GetEndpointByLocationId(locationId: 0); // request.LocationId.Value);
            
            SetContentType( ContentType.ApplicationWWWFormUrlEncodedAsUtf8 );
            SetEndpoint( locationEndpoint );
            SetMethod( typeof(TRequest).Name);

            var requestBody = RequestFilter(request);
            
            var response = await HostContext.Resolve<HttpClient>().SendStringToUrlAsync(
                url: _endpoint.ToString(),
                method: "POST",
                requestBody: requestBody,
                contentType: _contentType
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
        var baseUrl = new Uri("http://70.93.249.182");
        var result =  new UriBuilder(baseUrl.Scheme, baseUrl.Host, baseUrl.Port, "/cgi-bin/mgw.cgi").Uri;
        //var baseUrl = base.GetEndpointByLocationId(locationId);
        
        return result;
    }

}

public interface IE3CommandResult 
{
    
}

public abstract class E3CommandRequest<TRequest, TResponse, TResult> : E3Command<TRequest, TResponse, TResult> 
    where TRequest : E3CommandRequest<TRequest, TResponse, TResult> 
    where TResponse : E3CommandResponse<TResult> 
    where TResult : class, IHasResultData, new()
{
    [DataMember(Name = "id")] 
    [Input(Type = Input.Types.Hidden)]
    public string Id { get; set; } = new Random().Next(1000000000, 2000000000).ToString();

    [DataMember(Name = "jsonrpc")] 
    [Input(Type = Input.Types.Hidden)]
    public string JsonRPC { get; set; } = "2.0";

    [DataMember(Name = "method")] 
    [Input(Type = Input.Types.Hidden)]
    public string Method { get; set; } = typeof(TRequest).Name;
    
    [DataMember(Name = "params")] 
    [Input(Type = Input.Types.Hidden)]
    public List<object>? Parameters { get; set; }

    protected E3CommandRequest(params object[] parameters)
    {
        if (parameters.Length > 0)
            Parameters.AddRange(parameters);
    }
    
    public override string ToString()
    {
        return this.ToJson();
    }
}

public class E3CommandResponse<TResult> : IHasResult<TResult> where TResult : class, new()
{
    [DataMember(Name="id")]
    public string Id { get; set; }

    [DataMember(Name="jsonrpc")]
    public string JsonRPC { get; set; }

    [DataMember(Name = "result")] 
    public TResult Result { get; set; } = new();
}
