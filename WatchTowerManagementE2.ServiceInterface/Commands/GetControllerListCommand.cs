using ServiceStack;
using ServiceStack.Html;
using WatchTowerManagementE2.ServiceInterface.Commands.Types;
using WatchTowerManagementE2.ServiceModel;
using WatchTowerManagementE2.ServiceModel.Commands.GetControllerList;

namespace WatchTowerManagementE2.ServiceInterface.Commands;

public class GetControllerListCommand : E2Command<GetControllerList, List<Controller>>
{
    [Input(Type=Input.Types.Text, Value = "10753")]
    public int LocationId { get; set; }

    public override async Task ExecuteAsync(GetControllerList request)
    {
        var result = await ExecuteE2Command<GetControllerListResult>
        (
            GetEndpointByLocationId(request.LocationId), 
            "E2.GetControllerList", 
            json => json.FromJson<GetControllerListResult>(),
            [1] 
        ); 
        
        Result = result.Controllers;
    }
}