using ServiceStack;
using ServiceStack.DataAnnotations;
using WatchTowerManagementE2.ServiceModel.Types;

namespace WatchTowerManagementE2.ServiceModel;

[Tag("Reference Data")]
public class GetApplicationTypes : IReturn<GetApplicationTypesResponse>, IGet
{
}

public class GetApplicationTypesResponse : IHasResult<List<ApplicationType>>, IHasResponseStatus
{
    public List<ApplicationType> Result { get; set; } = new();
    public ResponseStatus ResponseStatus { get; set; }
}

[Tag("E2"), Description("Find Application Types")]
[Route("/query/application-types", "GET")]
public class QueryApplicationTypes : QueryDataSource<ApplicationType>
{
    public QueryApplicationTypes(QueryDataContext context) : base(context)
    {
    }

    public override IEnumerable<ApplicationType> GetDataSource(IDataQuery q)
    {
        var response = HostContext.AppHost.GetServiceGateway().Send(new GetApplicationTypes());

        return response.Result;
    }
}
