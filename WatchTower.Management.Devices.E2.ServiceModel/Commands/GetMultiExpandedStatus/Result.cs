using System.Runtime.Serialization;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus;

public class Result
{
    [DataMember(Name="Data")]
    public List<MultiExpandStatus> Results { get; set; }
}