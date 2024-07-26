using System.Runtime.Serialization;

namespace WatchTowerManagementE2.ServiceModel.Commands.GetCellList;

public class Result
{
    [DataMember(Name="Data")]
    public List<Cell> Cells { get; set; }
}