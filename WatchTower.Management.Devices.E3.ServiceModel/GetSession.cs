using ServiceStack;
using WatchTower.Management.Devices.E3.ServiceModel.Types.GetSession;

namespace WatchTower.Management.Devices.E3.ServiceModel;

[Tag("E3 - 2. Commands")]
public class GetSessionID : E3CommandRequest<GetSessionID, GetSessionIDResponse, GetSessionIDResult>
{
    protected override string RequestFilter(GetSessionID request)
    {
        var _payload = request.ToObjectDictionary();
        var parametersKey = nameof(request.Parameters);
        
        if (_payload.ContainsKey(parametersKey))
        {
            var parameters = (List<object>?)_payload[parametersKey];

            if (parameters is not null && !parameters.Any())
                _payload.Remove(parametersKey);
        }

        var __payload = _payload.FromObjectDictionary<GetSessionID>();
        
        return $"m={__payload.ToJson()}";
    }

    protected override GetSessionIDResult ResponseFilter(string json)
    {
        Result = json.FromJson<GetSessionIDResult>();

        return Result;
    }
}

public class GetSessionIDResponse : E3CommandResponse<GetSessionIDResult>
{
    public GetSessionIDResult Result { get; set; }
}

