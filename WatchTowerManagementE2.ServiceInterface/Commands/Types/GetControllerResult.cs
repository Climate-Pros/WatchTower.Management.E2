using System.Runtime.Serialization;
using WatchTowerManagementE2.ServiceModel.Commands.GetControllerList;

namespace WatchTowerManagementE2.ServiceInterface.Commands.Types;

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