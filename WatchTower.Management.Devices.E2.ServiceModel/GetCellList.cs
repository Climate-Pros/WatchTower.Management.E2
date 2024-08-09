using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;
using WatchTower.Management.Devices.E2.ServiceModel.Types;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2 - 2. Commands")]
public class GetCellList : E2CommandRequest<GetCellList, GetCellListResponse, GetCellListResult>
{
    protected override string RequestFilter(GetCellList request)
    {
        var result = CreatePayload
        (
            [1]
        );

        return result.ToJson();
    }

    protected override GetCellListResult ResponseFilter(string json)
    {
        return json.FromJson<GetCellListResult>();
    }

    public string? ControllerName { get; set; }
}

public class GetCellListResponse : E2CommandResponse<GetCellListResult>
{
    public GetCellListResult Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}