using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus;
using WatchTower.Management.Devices.E2.ServiceModel.Types;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2 - 2. Commands")]
public class GetMultiExpandedStatus : E2CommandRequest<GetMultiExpandedStatus, GetMultiExpandedStatusResponse , GetMultiExpandedStatusResult>
{
    public int LocationId { get;set; }
    
    public string ControllerName { get; set; }
    public List<string> Points { get; set; } = new();

    protected override string RequestFilter(GetMultiExpandedStatus request)
    {
        var result = new
        {
            Params = new List<dynamic> { new[]{ 1 } }
        };

        return result.ToJson();
    }

    protected override GetMultiExpandedStatusResult ResponseFilter(string json)
    {
        return json.FromJson<GetMultiExpandedStatusResult>();
    }

}

public class GetMultiExpandedStatusResponse :  E2CommandResponse<GetMultiExpandedStatusResult>
{
    public string Stopwatch { get; set; }
    public GetMultiExpandedStatusResult Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}