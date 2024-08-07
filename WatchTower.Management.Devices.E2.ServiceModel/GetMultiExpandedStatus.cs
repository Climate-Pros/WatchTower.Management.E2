using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("Controller Data")]
public class GetMultiExpandedStatus : IReturn<GetMultiExpandedStatusResponse>
{
    public int LocationId { get;set; }
    public string ControllerName { get; set; }
    public List<string> Points { get; set; } = new();
}

public class GetMultiExpandedStatusResponse : IHasResult<List<List<MultiExpandStatus>>>, IHasResponseStatus
{
    public string Stopwatch { get; set; }
    public List<List<MultiExpandStatus>> Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}