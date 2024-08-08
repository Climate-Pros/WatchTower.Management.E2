using Microsoft.CodeAnalysis.Options;
using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;
using WatchTower.Management.Devices.Shared;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2 - 2. Commands")]
public class GetCellList : DeviceCommand<GetCellList, GetCellListResponse, GetCellListResult>
{
    public int LocationId { get; set; }
    public string ControllerName { get; set; }
}

public class GetCellListResponse : IHasResult<GetCellListResult>
{
    public GetCellListResult Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}