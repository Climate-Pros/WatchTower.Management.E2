using System.Runtime.Serialization;
using WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;

public class Result : IHasResultData
{
    [DataMember(Name="data")]
    public List<Cell> Data { get; set; }
}