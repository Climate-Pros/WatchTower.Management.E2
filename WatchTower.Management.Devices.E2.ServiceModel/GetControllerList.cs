using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetControllerList;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("Controller Data")]
public class GetControllerList : IReturn<GetControllerListResponse>
{
    public int LocationId { get; set; }
}

public class GetControllerListResponse : IHasResult<List<Controller>>, IHasResponseStatus
{
    public List<Controller> Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}