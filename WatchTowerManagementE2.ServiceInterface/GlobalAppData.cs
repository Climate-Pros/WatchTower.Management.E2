using WatchTowerManagementE2.ServiceInterface.Commands.Types;

namespace WatchTowerManagementE2.ServiceInterface;

public class GlobalAppData
{
	public static GlobalAppData Instance { get; private set; } = new();

	public List<Location> AllLocations              { get; set; } = new();
	public List<string> AllLocationNames { get; set; } = new();
}
