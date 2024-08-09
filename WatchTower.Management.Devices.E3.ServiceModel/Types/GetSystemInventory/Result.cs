using System.Runtime.Serialization;

namespace WatchTower.Management.Devices.E3.ServiceModel.Types.GetSystemInventory;

public class Result
{
    [DataMember(Name="aps")]
    public List<Ap> Aps { get; set; }
}