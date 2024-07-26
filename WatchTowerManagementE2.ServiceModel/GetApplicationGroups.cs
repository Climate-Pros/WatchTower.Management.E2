using ServiceStack;

namespace WatchTowerManagementE2.ServiceModel;

[Tag("Reference Data")]
public class GetApplicationGroups : IReturn<GetApplicationGroupsResponse>
{
}

public class GetApplicationGroupsResponse : IHasResult<Dictionary<string, List<string>>>, IHasResponseStatus
{
    public Dictionary<string, List<string>> Result { get; set; } = new();
    public ResponseStatus ResponseStatus { get; set; }
}