using System.Runtime.Serialization;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;

public class Result
{
    [DataMember(Name="Data")]
    public List<Cell> Cells { get; set; }
}