using ServiceStack;
using WatchTower.Management.Devices.E3.ServiceModel;
using WatchTower.Management.Devices.E3.ServiceModel.Types.GetSystemInventory;
using WatchTower.Management.Devices.Shared;

namespace WatchTower.Management.Devices.E3.ServiceModel;

public class GetSystemInventory : E3Command<GetSystemInventory, GetSystemInventoryResponse, GetSystemInventoryResult>
{

    protected GetSystemInventory(int locationId) : base(locationId)
    {
        LocationId = locationId;
    }
    
    protected override GetSystemInventoryResult ResponseFilter(string json)
    {
        return json.FromJson<GetSystemInventoryResult>();
    }
}

public class GetSystemInventoryResponse : IHasResult<GetSystemInventoryResult>
{
    public GetSystemInventoryResult Result { get; set; }
}

public class GetSystemInventoryCommand : GetSystemInventory
{
    /*public override async Task<GetSystemInventoryResult> ExecuteCommand
    (
        Uri getEndpointByLocationId, 
        string getSystemInventoryName, 
        GetSystemInventory parameters, 
        Func<string, GetSystemInventoryResult> results
    ) => Result = base.CommandExecutor(
    {
        GetEndpointByLocationId(LocationId),
        nameof(GetSystemInventory),
        null,
        json => json.FromJson<GetSystemInventoryResult>()
    }*/

    
    public GetSystemInventoryCommand(int locationId) : base(locationId)
    {
        LocationId = locationId;
    }

    /*
    protected override async Task<GetSystemInventoryResult> ExecuteCommand(GetSystemInventory request, Func<string, GetSystemInventoryResult> responseFilter)
    {
        
    }
    */

    protected override GetSystemInventoryResult ResponseFilter(string json)
    {
        return json.FromJson<GetSystemInventoryResult>();
    }
}