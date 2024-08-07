using System.Runtime.Serialization;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands.GetControllerList;

public class GetControllerListResult
{
    [DataMember(Name="Result")]
    public List<Controller> Controllers { get; set; }

    public int Id { get; set; }
}