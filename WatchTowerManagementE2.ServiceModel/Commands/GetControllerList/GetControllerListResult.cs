using System.Runtime.Serialization;

namespace WatchTowerManagementE2.ServiceModel.Commands.GetControllerList;

public class GetControllerListResult
{
    [DataMember(Name="Result")]
    public List<Controller> Controllers { get; set; }

    public int Id { get; set; }
}