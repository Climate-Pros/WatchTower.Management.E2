using ServiceStack;
using WatchTower.Management.Devices.E2.ServiceModel.Types;

namespace WatchTower.Management.Devices.E2.ServiceModel.Commands;

public static class Types
{
    private static List<ApplicationType>? _applicationTypes = default;
    private static List<ApplicationGroup>? _applicationGroups = default;
    
    public static  List<ApplicationType> ApplicationTypes
    {
        get
        {
            if (_applicationTypes is null)
                _applicationTypes = ((GetApplicationTypesResponse)HostContext.AppHost.ExecuteService(new GetApplicationTypes())).Result;

            return _applicationTypes;
        }
    }

    public static List<ApplicationGroup> ApplicationGroups
    {
        get
        {
            if (_applicationGroups is null)
                _applicationGroups =  ((GetApplicationGroupsResponse)HostContext.AppHost.ExecuteService(new GetApplicationGroups())).Result;

            return _applicationGroups;
        }
    }

}