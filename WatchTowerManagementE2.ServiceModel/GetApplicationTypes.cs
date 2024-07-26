using ServiceStack;

namespace WatchTowerManagementE2.ServiceModel;

[Tag("Reference Data")]
public class GetApplicationTypes : IReturn<GetApplicationTypesResponse>
{
}

public class GetApplicationTypesResponse : IHasResult<List<KeyValuePair<string, string>>>, IHasResponseStatus
{
    public List<KeyValuePair<string, string>> Result { get; set; } = new();
    public ResponseStatus ResponseStatus { get; set; }
}