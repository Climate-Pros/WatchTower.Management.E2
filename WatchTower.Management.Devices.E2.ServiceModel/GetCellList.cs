using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;
using WatchTower.Management.Devices.E2.ServiceModel.Types;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2 - 2. Commands")]
public class GetCellList : E2Command<GetCellList, GetCellListResponse, GetCellListResult>
{
    protected override GetCellListResult ResponseFilter(string json)
    {
        return json.FromJson<GetCellListResult>();
    }

    public string? ControllerName { get; set; }
}

public class GetCellListResponse : IHasResult<GetCellListResult>
{
    public GetCellListResult Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}