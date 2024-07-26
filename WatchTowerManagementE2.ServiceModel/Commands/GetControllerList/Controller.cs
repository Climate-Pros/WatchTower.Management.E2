namespace WatchTowerManagementE2.ServiceModel.Commands.GetControllerList;

public class Controller
{
    public string Name { get; set; }
    public int Type { get; set; }
    public string Model { get; set; }
    public string Revision { get; set; }
    public int Subnet { get; set; }
    public int Node { get; set; }
}