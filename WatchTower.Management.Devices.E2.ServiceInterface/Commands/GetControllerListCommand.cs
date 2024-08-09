using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetControllerList;

namespace WatchTower.Management.Devices.E2.ServiceInterface.Commands;

public class GetControllerListCommand :  GetControllerList
{
    public override async Task ExecuteAsync(GetControllerList request)
    {
        await base.ExecuteAsync(request);
    }
}