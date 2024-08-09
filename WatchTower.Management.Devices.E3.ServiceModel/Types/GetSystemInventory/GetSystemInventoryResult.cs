using System.Runtime.Serialization;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

namespace WatchTower.Management.Devices.E3.ServiceModel.Types.GetSystemInventory;

public class GetSystemInventoryResult : IHasResultData
{
    [DataMember(Name="jsonrpc")]
    public string Jsonrpc { get; set; }

    [DataMember(Name="result")]
    public Result Result { get; set; }

    [DataMember(Name="id")]
    public string Id { get; set; }
}