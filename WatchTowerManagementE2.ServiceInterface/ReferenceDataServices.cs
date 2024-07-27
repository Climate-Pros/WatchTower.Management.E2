using ServiceStack;
using ServiceStack.Configuration;
using WatchTowerManagementE2.ServiceModel;
using WatchTowerManagementE2.ServiceModel.Types;

namespace WatchTowerManagementE2.ServiceInterface;

public class ReferenceDataServices : Service
{
    public IAppSettings AppSettings => HostContext.AppSettings;

    public object Any(GetApplicationTypes request)
    {
        return new GetApplicationTypesResponse
        {
            Result = AppSettings.GetDictionary("ReferenceData:ApplicationTypes").Select(type => new ApplicationType
            { 
                Type = type.Value,
                Name = type.Key
            }
            ).ToList() 
            
        };
    }
    
    public object Any(GetApplicationGroups request)
    {
        return new GetApplicationGroupsResponse 
        {   
            Result = AppSettings.Get<Dictionary<string, List<string>>>("ReferenceData:ApplicationGroups").Select(group => new ApplicationGroup
            {
                Name = group.Key, 
                Variables = group.Value
            })
            .ToList()
        };
    }

}