namespace WatchTower.Management.Devices.E2.ServiceModel.Interfaces;

public interface IController
{
    string Name { get; set; }
    int Type { get; set; }
    string Model { get; set; }
    string Revision { get; set; }
    int Subnet { get; set; }
    int Node { get; set; }
}