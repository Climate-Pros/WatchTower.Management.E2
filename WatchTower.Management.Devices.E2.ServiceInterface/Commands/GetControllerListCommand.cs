using Microsoft.CodeAnalysis.Options;
using ServiceStack;
using ServiceStack.Html;
using WatchTower.Management.Devices.E2.ServiceModel;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetControllerList;
using WatchTower.Management.Devices.Shared;

namespace WatchTower.Management.Devices.E2.ServiceInterface.Commands;

public class GetControllerListCommand : GetControllerList
{
    [Input(Type=Input.Types.Text, Value = "10753")]
    public int LocationId { get; set; }

    public override async Task ExecuteAsync(GetControllerList request)
    {
        var response = await ExecuteCommand
        (
           endpoint: GetEndpointByLocationId(request.LocationId), 
            method: "E2.GetControllerList", 
            parameters: [1],
            json => json.FromJson<GetControllerListResult>()
        ); 
        
        Result = response;
    }
}