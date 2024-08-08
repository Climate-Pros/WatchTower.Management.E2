using System.Runtime.Serialization;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

namespace WatchTower.Management.Devices.Shared.Types;

[DataContract]
public class Controllers
{
    public Controllers()
    {
    }

    [DataMember(Name="id")]
    public int Id { get; set; }

    [DataMember(Name = "result")]
    public List<IController>? Results { get; set; }
}