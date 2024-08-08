using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;

public class GetCellListResult : IHasResultData<Result>
{
    public Result Result { get; set; }
}