using System.Runtime.Serialization;

namespace WatchTowerManagementE2.ServiceModel.Commands.GetMultiExpandedStatus;

public class Result
{
    [DataMember(Name="Data")]
    public List<MultiExpandStatus> Results { get; set; }
}