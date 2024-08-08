using Microsoft.CodeAnalysis.Options;
using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;
using WatchTower.Management.Devices.Shared;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2")]
public class GetCellList : IReturn<GetCellListResponse>
{
    public int LocationId { get; set; }
    public string ControllerName { get; set; }
}

public class GetCellListResponse : IHasResult<GetCellListResult>
{
    public GetCellListResult Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}