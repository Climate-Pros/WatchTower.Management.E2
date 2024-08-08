using Microsoft.CodeAnalysis.Options;
using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetControllerList;
using WatchTower.Management.Devices.Shared;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2 - Commands")]
public class GetControllerList : DeviceCommand<GetControllerList, GetControllerListResponse, GetControllerListResult>
{
    public int LocationId { get; set; }
}

public class GetControllerListResponse : IHasResult<GetControllerListResult>
{
    public GetControllerListResult Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}