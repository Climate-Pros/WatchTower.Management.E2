using Microsoft.CodeAnalysis.Options;
using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus;
using WatchTower.Management.Devices.Shared;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2 - Commands")]
public class GetMultiExpandedStatus : DeviceCommand<GetMultiExpandedStatus, GetMultiExpandedStatusResponse , GetMultiExpandedStatusResult>
{
    public int LocationId { get;set; }
    public string ControllerName { get; set; }
    public List<string> Points { get; set; } = new();
}

public class GetMultiExpandedStatusResponse :  IHasResult<GetMultiExpandedStatusResult>
{
    public string Stopwatch { get; set; }
    public GetMultiExpandedStatusResult Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}