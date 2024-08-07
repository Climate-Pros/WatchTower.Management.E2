using System.Runtime.Serialization;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetControllerList;

namespace WatchTower.Management.Devices.E2.ServiceInterface.Commands.Types;

[DataContract]
public class GetControllerResult
{
    public GetControllerResult()
    {
    }

    [DataMember(Name="id")]
    public int Id { get; set; }

    [DataMember(Name = "result")]
    public List<Controller>? Results { get; set; }
}