using ServiceStack;
using WatchTowerManagementE2.ServiceModel.Types;

namespace WatchTowerManagementE2.ServiceModel;

[Tag("Reference Data")]
public class GetApplicationGroups : IReturn<GetApplicationGroupsResponse>
{
}

public class GetApplicationGroupsResponse : IHasResult<List<ApplicationGroup>>, IHasResponseStatus
{
    public List<ApplicationGroup> Result { get; set; } = new();
    public ResponseStatus ResponseStatus { get; set; }
}