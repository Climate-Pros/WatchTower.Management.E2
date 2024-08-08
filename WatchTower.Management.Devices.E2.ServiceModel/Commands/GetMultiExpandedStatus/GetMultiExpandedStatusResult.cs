using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus;

public class GetMultiExpandedStatusResult : IHasResultData
{
    public Result Result { get; set; }
    public int Id { get; set; }
}