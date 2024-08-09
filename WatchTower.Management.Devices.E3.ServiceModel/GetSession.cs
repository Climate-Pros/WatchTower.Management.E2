using ServiceStack;
using WatchTower.Management.Devices.E3.ServiceModel;
using WatchTower.Management.Devices.E3.ServiceModel.Types.GetSession;

namespace WatchTower.Management.Devices.E3.ServiceModel;

[Tag("E3 - 2. Commands")]
public class GetSession : E3Command<GetSession, GetSessionResponse, GetSessionResult>
{
    protected GetSession(int locationId) : base(locationId)
    {
    }

    protected override GetSessionResult ResponseFilter(string json)
    {
        return json.FromJson<GetSessionResult>();
    }
}

public class GetSessionResponse : E3CommandResponse<GetSessionResult>
{
    public GetSessionResult Result { get; set; }
}

public class GetSessionCommand : GetSession
{
    public GetSessionCommand(int locationId) : base(locationId)
    {
    }
}