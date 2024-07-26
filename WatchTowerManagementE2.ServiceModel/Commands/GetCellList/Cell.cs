namespace WatchTowerManagementE2.ServiceModel.Commands.GetCellList;

public class Cell
{
    public string Controller { get; set; }
    public string CellName { get; set; }
    public string CellLongName { get; set; }
    public int CellType { get; set; }
    public string CellTypeName { get; set; }
}