using System.Runtime.Serialization;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands.GetControllerList;

public class GetControllerListResult : IHasResultData<List<Controller>>
{
    [DataMember(Name = "Result")] 
    public List<Controller> Result { get; set; } = new();
}