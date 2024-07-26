using ServiceStack;
using WatchTowerManagementE2.ServiceModel.Commands.GetCellList;

namespace WatchTowerManagementE2.ServiceModel;

[Tag("Controller Data")]
public class GetCellList() : IReturn<GetCellListResponse>
{
    public int LocationId { get; set; }
    public string ControllerName { get; set; }
}

public class GetCellListResponse : IHasResult<List<Cell>>, IHasResponseStatus
{
    public List<Cell> Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}