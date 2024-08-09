using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;

namespace WatchTower.Management.Devices.E2.ServiceInterface.Commands;

public class GetCellListCommand :  GetCellList
{
    public override async Task ExecuteAsync(GetCellList request)
    {
        await base.ExecuteAsync(request);
    }
}