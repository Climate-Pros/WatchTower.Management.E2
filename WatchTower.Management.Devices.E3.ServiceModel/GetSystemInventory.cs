using ServiceStack;
using WatchTower.Management.Devices.E3.ServiceModel.Types.GetSystemInventory;

namespace WatchTower.Management.Devices.E3.ServiceModel;

[Tag("E3 - 2. Commands")]
public class GetSystemInventory : E3CommandRequest<GetSystemInventory, GetSystemInventoryResponse, GetSystemInventoryResult>
{
    protected override string RequestFilter(GetSystemInventory request)
    {
        return $"m={request.ToJson()}";
    }

    protected override GetSystemInventoryResult ResponseFilter(string json)
    {
        return json.FromJson<GetSystemInventoryResult>();
    }
}

public class GetSystemInventoryResponse : E3CommandResponse<GetSystemInventoryResult>
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

    protected override GetSystemInventoryResult ResponseFilter(string json)
    {
        return json.FromJson<GetSystemInventoryResult>();
    }
}