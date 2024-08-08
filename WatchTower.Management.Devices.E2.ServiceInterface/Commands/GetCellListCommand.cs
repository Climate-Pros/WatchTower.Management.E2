using ServiceStack;
using ServiceStack.Html;
using WatchTower.Management.Devices.E2.ServiceModel;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;
using WatchTower.Management.Devices.Shared;

namespace WatchTower.Management.Devices.E2.ServiceInterface.Commands;

public class GetCellListCommand :  DeviceCommand<GetCellList, GetCellListResponse, GetCellListResult>
{
    public override async Task ExecuteAsync(GetCellList request) => Result = await ExecuteCommand
    (
        GetEndpointByLocationId(request.LocationId),
        "E2.GetCellList",
        [ request.ControllerName ],
        json => json.FromJson<GetCellListResult>()
    );

    public int LocationId { get; set; }
    public string ControllerName { get; set; }
}