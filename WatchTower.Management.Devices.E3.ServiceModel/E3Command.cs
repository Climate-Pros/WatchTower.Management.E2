using System.Runtime.Serialization;
using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;
using WatchTower.Management.Devices.Shared;
using WatchTower.Management.Devices.Shared.Enums;
using WatchTower.Management.Devices.Shared.Types;

namespace WatchTower.Management.Devices.E3.ServiceModel;

[DataContract(Name="Params")]
public class E3CommandParameters
{
    [DataMember(Name="sid")]
    public string Sid { get; set; }
}

public abstract class E3Command<TRequest, TResponse, TResult> : DeviceCommand<TRequest, TResponse, TResult> 
    where TRequest : DeviceCommand<TRequest, TResponse, TResult>
    where TResponse : class, IHasResult<TResult> 
    where TResult : class, IHasResultData, new()
{
    private long minId = long.Parse(int.MaxValue.ToString()) - 1000000000;
    private long maxId = long.Parse(int.MaxValue.ToString());
    
    [DataMember(Name = "id")] 
    public string Id => new Random().NextInt64(minId, maxId).ToString();

    [DataMember(Name = "jsonrpc")] 
    public string Jsonrpc { get; set; } = "2.0";

    [DataMember(Name = "method")] 
    public string Method => _method;

    [DataMember(Name = "params")] 
    public E3CommandParameters Params { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    
    public E3Command(int locationId)
    {
        LocationId = locationId;
    }

    protected override void SetMethod(string? value)
    {
        if (value is not null)
            _method = value;

        _method = typeof(TRequest).Name;
    }

    protected override string CreatePayload(TRequest request)
    {
        return $"m={request.ToJson()}";
    }

    public override async Task ExecuteAsync(TRequest request)
    {
        try
        {
            var locationEndpoint = GetEndpointByLocationId(locationId: request.LocationId);
            
            SetContentType( ContentType.TextPlain );
            SetEndpoint( locationEndpoint );
            SetMethod( $"E2.{nameof(TRequest)}");
            
            var response = await HostContext.Resolve<HttpClient>().SendStringToUrlAsync(
                url: _endpoint.ToString(),
                method: _method,
                requestBody: CreatePayload(request),
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
        return new($"{base.GetEndpointByLocationId(locationId)}/cgi-bin/mgw.cgi");
    }

}

public interface IE3CommandResult 
{
    
}

public class E3CommandResponse<TResult> : IHasResult<TResult> where TResult : class, IE3CommandResult, new()
{
    [DataMember(Name="id")]
    public string Id { get; set; }

    [DataMember(Name="jsonrpc")]
    public string JsonRPC { get; set; }

    [DataMember(Name = "result")] 
    public TResult Result { get; set; } = new();
}
