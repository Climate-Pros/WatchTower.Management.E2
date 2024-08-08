using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Types;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2 - 1. Reference Data")]
public class GetApplicationGroups : IReturn<GetApplicationGroupsResponse>
{
}

public class GetApplicationGroupsResponse : IHasResult<List<ApplicationGroup>>, IHasResponseStatus
{
    public List<ApplicationGroup> Result { get; set; } = new();
    public ResponseStatus ResponseStatus { get; set; }
}