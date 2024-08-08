using System.Runtime.Serialization;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus;

public class Result : IHasData<List<MultiExpandStatus>>
{
    [DataMember(Name = "Data")]
    public List<MultiExpandStatus> Data { get; set; } = new();
}