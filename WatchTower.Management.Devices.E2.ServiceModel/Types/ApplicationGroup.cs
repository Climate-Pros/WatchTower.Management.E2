namespace WatchTower.Management.Devices.E2.ServiceModel.Types;

public class ApplicationGroup
{
    public string Name { get; set; }
    public List<string> Variables { get; set; } = new();
}