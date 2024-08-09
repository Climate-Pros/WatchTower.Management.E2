using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus;
using WatchTower.Management.Devices.E2.ServiceModel.Types;
using WatchTower.Management.Devices.Shared;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2 - 2. Commands")]
public class GetMultiExpandedStatus : E2Command<GetMultiExpandedStatus, GetMultiExpandedStatusResponse , GetMultiExpandedStatusResult>
{
    public int LocationId { get;set; }
    
    public string ControllerName { get; set; }
    public List<string> Points { get; set; } = new();

    protected override GetMultiExpandedStatusResult ResponseFilter(string json)
    {
        return json.FromJson<GetMultiExpandedStatusResult>();
    }

}

public class GetMultiExpandedStatusResponse :  IHasResult<GetMultiExpandedStatusResult>
{
    public string Stopwatch { get; set; }
    public GetMultiExpandedStatusResult Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}