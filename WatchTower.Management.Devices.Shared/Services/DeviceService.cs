using ServiceStack;
using static ServiceStack.HostContext;

namespace WatchTower.Management.Devices.Shared.Services;

public class DeviceService : Service
{
    /// <summary>
    /// 
    /// </summary>
    protected ICommandExecutor CommandExecutor => Resolve<ICommandExecutor>();

    protected DeviceService()
    {
        if (Container is not null)
            Container.AutoWire(this);
    }
    
}