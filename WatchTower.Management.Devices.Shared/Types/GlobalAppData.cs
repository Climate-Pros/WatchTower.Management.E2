namespace WatchTower.Management.Devices.Shared.Types;

public class GlobalAppData
{
	public static GlobalAppData Instance { get; private set; } = new();

	public List<Location> AllLocations              { get; set; } = new();
	public List<string> AllLocationNames { get; set; } = new();
}
