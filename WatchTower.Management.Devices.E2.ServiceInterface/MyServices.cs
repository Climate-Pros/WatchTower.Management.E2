using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceInterface.Commands;
using WatchTower.Management.Devices.E2.ServiceModel;

namespace WatchTower.Management.Devices.E2.ServiceInterface;

public class MyServices : Service
{
    public ICommandExecutor CommandExecutor => HostContext.Resolve<ICommandExecutor>();

    public async Task<object> Any(GetControllerList request)
    {
        var result = await CommandExecutor.ExecuteWithResultAsync(new GetControllerListCommand { LocationId = request.LocationId}, request);

        return new GetControllerListResponse
        {
            Result = result
        };
    }

    public async Task<object> Any(GetCellList request)
    {
        var result =  await CommandExecutor.ExecuteWithResultAsync(new GetCellListCommand {  LocationId = request.LocationId,  ControllerName = request.ControllerName }, request);

        return new GetCellListResponse
        {
            Result = result
        };
    }

}