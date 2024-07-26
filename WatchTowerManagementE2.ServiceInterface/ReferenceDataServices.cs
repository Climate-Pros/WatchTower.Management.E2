using ServiceStack;
using ServiceStack.Configuration;
using WatchTowerManagementE2.ServiceModel;

namespace WatchTowerManagementE2.ServiceInterface;

public class ReferenceDataServices : Service
{
    public IAppSettings AppSettings => HostContext.AppSettings;

    public async Task<object> Any(GetApplicationTypes request)
    {
        return new GetApplicationTypesResponse
        {
            Result = AppSettings.GetDictionary("ReferenceData:ApplicationTypes").ToList() 
            
        };
    }
    
    public async Task<object> Any(GetApplicationGroups request)
    {
        return new GetApplicationGroupsResponse { 
            
            Result = AppSettings.Get<Dictionary<string, List<string>>>("ReferenceData:ApplicationGroups")
            };
    }

}