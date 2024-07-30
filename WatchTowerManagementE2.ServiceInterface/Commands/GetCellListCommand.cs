using ServiceStack;
using ServiceStack.Html;
using WatchTowerManagementE2.ServiceInterface.Commands.Types;
using WatchTowerManagementE2.ServiceModel;
using WatchTowerManagementE2.ServiceModel.Commands.GetCellList;

namespace WatchTowerManagementE2.ServiceInterface.Commands;

public class GetCellListCommand : E2Command<GetCellList, List<Cell>>
{
    [Input(Type=Input.Types.Text, Value = "10753")]
    public int LocationId { get; set; }
        
    public override async Task ExecuteAsync(GetCellList request)
    {
        var result = await ExecuteE2Command
        (
            GetEndpointByLocationId(request.LocationId), 
            "E2.GetCellList", 
            json => json.FromJson<GetCellListResult>(), 
            [request.ControllerName])
        ;
        
        Result = result.Result.Cells;
    }
}