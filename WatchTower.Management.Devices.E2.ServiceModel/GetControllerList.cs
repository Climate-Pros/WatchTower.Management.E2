using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetControllerList;
using WatchTower.Management.Devices.E2.ServiceModel.Types;

namespace WatchTower.Management.Devices.E2.ServiceModel;

[Tag("E2 - 2. Commands")]
public class GetControllerList : E2CommandRequest<GetControllerList, GetControllerListResponse, GetControllerListResult>
{

    public override async Task ExecuteAsync(GetControllerList request)
    {
        ;
        Console.Write("----------");
        await base.ExecuteAsync(request);
    }

    protected override string RequestFilter(GetControllerList request)
    {
        var result  = CreatePayload(1);

        return result;
    }

    protected override GetControllerListResult ResponseFilter(string json)
    {
        return json.FromJson<GetControllerListResult>();
    }
}

public class GetControllerListResponse : E2CommandResponse<GetControllerListResult>
{
    public GetControllerListResult Result { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}