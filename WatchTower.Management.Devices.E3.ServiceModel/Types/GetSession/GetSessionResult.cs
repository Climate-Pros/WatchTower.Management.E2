using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

namespace WatchTower.Management.Devices.E3.ServiceModel.Types.GetSession;

public class GetSessionIDResult : IHasResultData, IE3CommandResult
{
    
    public string JsonRPC { get; set; }
    public Result Result { get; set; }
    public string Id { get; set; }
}