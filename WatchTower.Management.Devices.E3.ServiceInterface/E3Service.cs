using ServiceStack;
using WatchTower.Management.Devices.E3.ServiceModel;
using WatchTower.Management.Devices.Shared.Services;
using static ServiceStack.HostContext;

namespace WatchTower.Management.Devices.E3.ServiceInterface;

public class E3Service : DeviceService
{
    public ICommandExecutor CommandExecutor => Resolve<ICommandExecutor>();

    public async Task<object> Any(GetSessionID request)
    {
        var result = await CommandExecutor.ExecuteWithResultAsync(new GetSessionID { }, request);

        return new GetSessionIDResponse
        {
            Result = result
        };
    }
    
    public async Task<object> Any(GetSystemInventory request)
    {
        var result = await CommandExecutor.ExecuteWithResultAsync(new GetSystemInventoryCommand { }, request);

        return new GetSystemInventoryResponse
        {
            Result = result
        };
    }
}